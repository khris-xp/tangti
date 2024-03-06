using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;
using Microsoft.VisualBasic;
using DnsClient.Protocol;

namespace tangti.Services;

public class EnrollService
{

    private readonly IMongoCollection<Enroll> _enrollCollection;

    public EnrollService(
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting)
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString);

        var mongoDatabase =  mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName
        );

        _enrollCollection = mongoDatabase.GetCollection<Enroll>(
            "enroll"
        );
    }

    public async Task<List<Enroll>> GetAsync() => 
        await _enrollCollection.Find(_ => true).ToListAsync();
    
    public async Task<Enroll?> GetAsync(string id) => 
        await _enrollCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Enroll?> GetEventEnrollAsync(string EventId) =>
        await _enrollCollection.Find(x => x.EventID == EventId).FirstOrDefaultAsync();

    public async Task CreateAsync(Enroll newEnroll) =>
        await _enrollCollection.InsertOneAsync(newEnroll);
    
    public async Task UpdateAsync(string id,Enroll updatedEnroll) =>
        await _enrollCollection.ReplaceOneAsync(x => x.Id == id, updatedEnroll);

    public async Task DeleteAsync(string id) =>
        await _enrollCollection.DeleteOneAsync(x => x.Id == id);
}