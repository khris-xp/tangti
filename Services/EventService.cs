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
