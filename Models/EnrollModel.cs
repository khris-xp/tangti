using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Enroll
    {
        public class JoinUserData{
            [BsonElement("UserID")]
            public string UserID { get; set;}
            public DateTime join_date { get; set; }
            
            
            public JoinUserData(string id) {
                UserID = id;
                join_date = DateTime.UtcNow;
            }
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("EventID")]
        public required string EventID { get; set; }

        [BsonElement("Member")]
        public required int Member { get; set;}

        [BsonElement("MemberList")]
        public List<JoinUserData> MemberList { get; set;}

        public Enroll(){
            MemberList = new List<JoinUserData>();
            Member = MemberList.Count;
        }
    }
}
