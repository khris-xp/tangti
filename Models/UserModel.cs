using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? ImageProfile { get; set; }
        public string? Role { get; set; }
        public List<string> Enrolled { get; set; }
        public List<string> EventCreated { get; set; }

        public List<string> Liked { get; set; }

        [BsonElement("CreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; }

        public UserModel()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
            Enrolled = new List<string>();
            EventCreated = new List<string>();
            Liked = new List<string>();
        }
    }
}
