using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class EventService
{
    private readonly IMongoCollection<Event> _eventCollectins;

    public EventService(
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting)
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName);

        _eventCollectins = mongoDatabase.GetCollection<Event>(
            "events");
    }

    public async Task<List<Event>> GetAsync() =>
        await _eventCollectins.Find(_ => true).ToListAsync();

    public async Task<Event?> GetAsync(string id) =>
        await _eventCollectins.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Event newEvent) =>
        await _eventCollectins.InsertOneAsync(newEvent);

    public async Task UpdateAsync(string id, Event updatedEvent) =>
        await _eventCollectins.ReplaceOneAsync(x => x.Id == id, updatedEvent);

    public async Task DeleteAsync(string id) =>
        await _eventCollectins.DeleteOneAsync(x => x.Id == id);
}