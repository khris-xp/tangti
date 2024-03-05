using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? EventId { get; set; }
        public string? Createdby { get; set; }
        
        [BsonElement("CreateAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateAt { get; set; }

        public Report()
        {
            CreateAt = DateTime.UtcNow;
        }
    }
}