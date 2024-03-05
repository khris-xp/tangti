using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public required string Name { get; set; }

        [BsonElement("CreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; }

        public Category()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }
    }
}