using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;
using System.Security.Cryptography;
using System.Text;

namespace tangti.Services;

public class AuthService
{
    private readonly IMongoCollection<UserModel> _usersCollection;
    private static bool VerifyPasswordHash(string password, byte[]? passwordHash, byte[]? passwordSalt)
    {
        using var hmac = passwordSalt != null ? new HMACSHA512(passwordSalt) : new HMACSHA512();

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (passwordHash == null || computedHash[i] != passwordHash[i])
            {
                return false;
            }
        }

        return true;
    }

    public AuthService(
        IOptions<BlogDatabaseSetting> userDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<UserModel>(
            userDatabaseSettings.Value.UserCollectionName);
    }

    public async Task Login(string username, string password)
    {
        var user = await _usersCollection.Find(x => x.UserName == username).FirstOrDefaultAsync();

        if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            throw new Exception("Username or password is incorrect");
        }
    }

    public async Task<UserModel> Register(string username, string email, string password)
    {
        var hmac = new HMACSHA512();

        var user = new UserModel
        {
            UserName = username,
            Email = email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            PasswordSalt = hmac.Key,
        };

        await _usersCollection.InsertOneAsync(user);

        return user;
    }
}
