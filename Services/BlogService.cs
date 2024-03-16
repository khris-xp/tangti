using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class BlogService
{
    private readonly IMongoCollection<Blog> _blogsCollection;

    public BlogService(
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting)
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName);

        _blogsCollection = mongoDatabase.GetCollection<Blog>(
            "blogs");
    }


    public async Task<List<Blog>> GetAsync() =>
        await _blogsCollection.Find(_ => true).ToListAsync();

    public async Task<Blog?> GetAsync(string id) =>
        await _blogsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Blog newBlog) =>
        await _blogsCollection.InsertOneAsync(newBlog);

    public async Task UpdateAsync(string id, Blog updatedBlog) =>

        await _blogsCollection.ReplaceOneAsync(x => x.Id == id, updatedBlog);

    public async Task DeleteAsync(string id) =>
        await _blogsCollection.DeleteOneAsync(x => x.Id == id);
}