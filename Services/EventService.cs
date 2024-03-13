using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services
{
    public class EventService
    {
        private readonly IMongoCollection<Event> _eventCollections;
        private readonly ILogger<EventService> _logger;

        public EventService(
            IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting,
            ILogger<EventService> logger)
        {
            var mongoClient = new MongoClient(
                tangtiDatabaseSetting.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                tangtiDatabaseSetting.Value.DatabaseName);

            _eventCollections = mongoDatabase.GetCollection<Event>(
                "events");
            _logger = logger;
        }

        public async Task<List<Event>> GetAsync()
        {
            return await _eventCollections.Find(_ => true).ToListAsync();
        }

        public async Task<List<Event>> GetAsyncSearch(string searchString)
        {
            return await _eventCollections.Find(x => x.Title.Contains(searchString)).ToListAsync();
        }


        public async Task<List<Event>> GetPaganationAsync(int page = 1, int pageSize = 5, string searchString = "", string category = "")

        {
            var filterBuilder = Builders<Event>.Filter;
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(searchString))
            {
                filter &= filterBuilder.Where(x => x.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(category))
            {
                filter &= filterBuilder.Eq(x => x.Category, category);
            }

            return await _eventCollections.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<long> GetTotalCountAsync(string searchString = "")
        {
            var filter = Builders<Event>.Filter.Empty;
            if (!string.IsNullOrEmpty(searchString))
            {
                filter = Builders<Event>.Filter.Where(x => x.Title.Contains(searchString));
            }
            return await _eventCollections.Find(filter).CountDocumentsAsync();
        }

        public async Task<Event> GetAsync(string id)
        {
            return await _eventCollections.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Event newEvent)
        {
            _logger.LogInformation("Creating a new event: {@Event}", newEvent);
            await _eventCollections.InsertOneAsync(newEvent);
        }

        public async Task UpdateAsync(string id, Event updatedEvent)
        {
            _logger.LogInformation("Updating event with ID {ID}. New data: {@UpdatedEvent}", id, updatedEvent);
            await _eventCollections.ReplaceOneAsync(x => x.Id == id, updatedEvent);
        }

        public async Task DeleteAsync(string id)
        {
            await _eventCollections.DeleteOneAsync(x => x.Id == id);
        }
        public async Task<bool> isTimeClose(string id)
        {
            var target_event = await GetAsync(id);
            var now_date = DateTime.Now;
            if (now_date > target_event.EnrollDate.EndDate)
                return true;
            return false;
        }

        public async Task<bool> isTimeNotOpen(string id)
        {
            var target_event = await GetAsync(id);
            var now_date = DateTime.Now;
            if (now_date < target_event.EnrollDate.StartDate)
                return (true);
            return (false);
        }

        public async Task<bool> isEnrollTime(string id)
        {
            if (await isTimeClose(id) || await isTimeNotOpen(id))
                return (false);
            return (true);
        }

        public async Task<bool> isTouchLimit(string id, Enroll enroll)
        {
            var target_event = await GetAsync(id);
            if (enroll == null)
                return (true);
            if (enroll.Member >= target_event.EnrollLimit)
                return (true);
            return (false);

        }

		public async Task<bool> changeStatus(string id, string status)
		{
			var target_event = await GetAsync(id);
			if (target_event == null)
				return (false);
			target_event.Status = status;
			await UpdateAsync(id, target_event);
			return (true);
		}
    }
}
