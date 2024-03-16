using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class History
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("EventId")]
        public required string EventId { get; set; }

        [BsonElement("EventName")]
        public required string EventName { get; set; }


        [BsonElement("userId")]
        public required string UserId { get; set; }

        [BsonElement("JoinDate")]
        public DateTime JoinDate { get; set; }




    }
}