using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;
using Microsoft.Extensions.Logging; // Add this namespace

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


        public async Task<List<Event>> GetPaganationAsync(int page = 1, int pageSize = 5, string searchString = "",string Category = "")

        {
            var filter = Builders<Event>.Filter.Empty;
            if (!string.IsNullOrEmpty(searchString))
            {
                filter = Builders<Event>.Filter.Where(x => x.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(Category))
            {
                filter = Builders<Event>.Filter.Where(x => x.Category == Category);
            }
            
            return await _eventCollections.Find(filter).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();
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
    }
}
