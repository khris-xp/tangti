using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Like
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("EventId")]
        public required string EventId { get; set; }

        [BsonElement("Like")]
        public List<string> Likes { get; set; }

        [BsonElement("LikeAmount")]
        public int LikeAmount { get; set; }

        public Like()
        {
            Likes = new List<string>();
            LikeAmount = 0;
        }

    }
}