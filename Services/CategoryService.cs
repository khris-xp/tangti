using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class CategoryService
{
    private readonly IMongoCollection<Category> _categoriesCollection;

    public CategoryService(
        IOptions<TangtiDatabaseSetting> tangtiDatabaseSetting)
    {
        var mongoClient = new MongoClient(
            tangtiDatabaseSetting.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            tangtiDatabaseSetting.Value.DatabaseName);

        _categoriesCollection = mongoDatabase.GetCollection<Category>(
            "categories");
    }

    public async Task<List<Category>> GetAsync() =>
        await _categoriesCollection.Find(_ => true).ToListAsync();

    public async Task<Category?> GetAsync(string id) =>
        await _categoriesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Category newCategory) =>
        await _categoriesCollection.InsertOneAsync(newCategory);

    public async Task UpdateAsync(string id, Category updatedCategory) =>
        await _categoriesCollection.ReplaceOneAsync(x => x.Id == id, updatedCategory);

    public async Task DeleteAsync(string id) =>
        await _categoriesCollection.DeleteOneAsync(x => x.Id == id);

}