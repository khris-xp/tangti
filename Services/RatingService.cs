using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class RatingService
{
    private readonly IMongoCollection<Rating> _ratingCollection;

    public RatingService (
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting
    )
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString
        );

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName
        );

        _ratingCollection = mongoDatabase.GetCollection<Rating>(
            "rating"
        );
    }

    public async Task<List<Rating>> GetRatingsAsync() => 
        await _ratingCollection.Find(_ => true).ToListAsync();

    public async Task<Rating?> GetRatingAsync(string id) =>
        await _ratingCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    
    public async Task CreateAsync(Rating newRating)
    {
        await _ratingCollection.InsertOneAsync(newRating);
    }

    public async Task UpdateAsync(string id, Rating updateRating) =>
        await _ratingCollection.ReplaceOneAsync(x => x.Id == id, updateRating);

    public async Task DeleteAsync(string id) =>
        await _ratingCollection.DeleteOneAsync(x => x.Id == id);
}