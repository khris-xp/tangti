using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class BlogService
{
    private readonly IMongoCollection<Blog> _blogsCollection;

    public BlogService(
        IOptions<BlogDatabaseSetting> blogDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            blogDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            blogDatabaseSettings.Value.DatabaseName);

        _blogsCollection = mongoDatabase.GetCollection<Blog>(
            blogDatabaseSettings.Value.BlogCollectionName);
    }

    public async Task<List<Blog>> GetAsync() =>
        await _blogsCollection.Find(_ => true).ToListAsync();

    public async Task<Blog?> GetAsync(string id) =>
        await _blogsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Blog newBook) =>
        await _blogsCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Blog updatedBook) =>
        await _blogsCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task DeleteAsync(string id) =>
        await _blogsCollection.DeleteOneAsync(x => x.Id == id);
}