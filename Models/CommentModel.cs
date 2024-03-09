using Amazon.SecurityToken.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("EventId")]
        public string? EventId { get; set; }

        [BsonElement("Content")]
        public required string Content;

        [BsonElement("CreatedBy")]
        public required string CreatedBy { get; set; }

        [BsonElement("CreateAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateAt { get; set; }

        public Comment(){
            CreateAt = DateTime.UtcNow;
        }
    }
}