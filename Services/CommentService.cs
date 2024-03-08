using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class CommentService
{
    private readonly IMongoCollection<Comment> _commentCollection;

    public CommentService(IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting)
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString
        );

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName
        );

        _commentCollection = mongoDatabase.GetCollection<Comment>(
            "comments"
        );
    }

    public async Task<List<Comment>> GetCommentsAsync() =>
        await _commentCollection.Find(_ => true).ToListAsync();
    
    public async Task<Comment?> GetCommentAsync(string id) =>
        await _commentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Comment>> GetEventCommentsAsync(string eventId) =>
        await _commentCollection.Find(x => x.EventId == eventId).ToListAsync();

    public async Task CreateAsync(Comment newComment) =>
        await _commentCollection.InsertOneAsync(newComment);
    
    public async Task UpdateAsync(string id, Comment updatedComment) =>
        await _commentCollection.ReplaceOneAsync(x => x.Id == id, updatedComment);

    public async Task DeleteAsync(string id) =>
        await _commentCollection.DeleteOneAsync(x => x.Id == id);
}