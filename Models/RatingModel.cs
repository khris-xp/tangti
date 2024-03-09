using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Rating
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("EventId")]
        public required string EventId { get; set; }

        [BsonElement("UserId")]
        public required string UserId { get; set; }

        [BsonElement("RatingScore")]
        public required float RatingScore { get; set; }
    }
}