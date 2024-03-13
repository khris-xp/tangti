using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;
using Microsoft.VisualBasic;
using DnsClient.Protocol;

namespace tangti.Services;

public class HistoryService
{

    private readonly IMongoCollection<History> _historyCollection;

     private readonly ILogger<HistoryService> _logger;

    public HistoryService(
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting,
        ILogger<HistoryService> logger)
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName
        );

        _historyCollection = mongoDatabase.GetCollection<History>(
            "history"
        );

        _logger = logger;
    }

    public async Task<List<History>> GetAsync() =>
        await _historyCollection.Find(_ => true).ToListAsync();

    
    public async Task<List<History>> GetByUserIdAsync(string userId) =>
        await _historyCollection.Find(x => x.UserId == userId).ToListAsync();

    public async Task DeleteByEventIdAndUserIdAsync(string eventId, string userId) =>
        await _historyCollection.DeleteOneAsync(x => x.EventId == eventId && x.UserId == userId);


    public async Task<History?> GetAsync(string id) =>
        await _historyCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<History?> GetEventHistoryAsync(string HistoryId) =>
        await _historyCollection.Find(x => x.Id == HistoryId).FirstOrDefaultAsync();

    public async Task CreateAsync(History newHistory){
        try{
            _logger.LogInformation("Creating a new event: {@History}", newHistory);
            await _historyCollection.InsertOneAsync(newHistory);
            
        }catch(Exception e){
            _logger.LogError(e.Message);

        }


    }

    public async Task UpdateAsync(string id, History updatedHistory) =>
        await _historyCollection.ReplaceOneAsync(x => x.Id == id, updatedHistory);

    public async Task DeleteAsync(string id) =>
        await _historyCollection.DeleteOneAsync(x => x.Id == id);


}