using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Enroll
    {
        public class JoinUserData{
            public string Id { get; set;}
            public DateTime join_date { get; set; }
            
            public JoinUserData(string id) {
                Id = id;
                join_date = DateTime.UtcNow;
            }
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("EventID")]
        public required string EventID { get; set; }

        [BsonElement("member")]
        public required int member { get; set;}

        [BsonElement("member_list")]
        public required List<string> member_list { get; set;}

        [BsonElement("Join_date")]
        public List<JoinUserData> Joindate { get; set;}

        public Enroll(){
            Joindate = new List<JoinUserData>();
        }
    }
}
