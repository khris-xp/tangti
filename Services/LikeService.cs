using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class LikeService
{
    private readonly IMongoCollection<Like> _likeCollection;

    public LikeService(
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting
    )
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString
        );

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName
        );

        _likeCollection = mongoDatabase.GetCollection<Like>(
            "likes"
        );
    }

    public async Task<List<Like>> GetLikesAsync() =>
        await _likeCollection.Find(_ => true).ToListAsync();

    public async Task<Like?> GetLikeAsync(string id) =>
        await _likeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Like?> GetLikeEventAsync(string eventid) =>
        await _likeCollection.Find(x => x.EventId == eventid).FirstOrDefaultAsync();
    
    public async Task CreateAsync(Like newLike){
        await _likeCollection.InsertOneAsync(newLike);
    }

    public async Task UpdateAsync(string id, Like updateLike) =>
        await _likeCollection.ReplaceOneAsync(x => x.Id == id, updateLike);

    public async Task DeleteAsync(string id) =>
        await _likeCollection.DeleteOneAsync(x => x.Id == id);
}