using tangti.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using tangti.Configs;
using System.Security.Cryptography;
using System.Text;
using tangti.DTOs;

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
        IOptions<TangtiDatabaseSetting> userDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<UserModel>(
            "users");
    }

    public async Task<UserModel?> Login(string email, string password)
    {
        var user = await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            throw new Exception("Email or password is incorrect");
        }
        return user;
    }

    public async Task<UserModel> Register(RegisterDto user)
    {
        var hmac = new HMACSHA512();

        var new_user = new UserModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FirstName + " " + user.LastName,
            Phone = user.Phone,
            Email = user.Email,
            Role = "user",
            Enrolled = user.Enrolled,
            EventCreated = user.EventCreated,
            ImageProfile = user.ImageProfile,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
            PasswordSalt = hmac.Key,
        };

        await _usersCollection.InsertOneAsync(new_user);

        return new_user;
    }
}
