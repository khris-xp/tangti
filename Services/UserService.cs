using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;

namespace tangti.Services;

public class UserService
{
    private readonly IMongoCollection<UserModel> _usersCollection;
    private readonly IMongoCollection<Event> _eventCollection;

    public UserService(
        IOptions<TangtiDatabaseSetting> userDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<UserModel>(
            "users");
        _eventCollection = mongoDatabase.GetCollection<Event>(
            "events");
    }

    public async Task<List<UserModel>> GetUsersAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<UserModel?> GetUserAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<UserModel?> GetUserByEmail(string email) =>
        await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

    public async Task<UserModel> CreateUserAsync(UserModel user)
    {
        await _usersCollection.InsertOneAsync(user);
        return user;
    }
    public async Task<Event> GetUserEventsAsync(string id) =>
        await _eventCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task UpdateUserAsync(string id, UserModel userIn) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, userIn);

    public async Task DeleteUserAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);

}