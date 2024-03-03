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

        [BsonElement("Member")]
        public required int Member { get; set;}

        [BsonElement("Member_list")]
        public required List<string> Member_list { get; set;}

        [BsonElement("Join_dates")]
        public List<JoinUserData> Joindate { get; set;}

        public Enroll(){
            Member_list = new List<string>();
            Joindate = new List<JoinUserData>();
            Member = Member_list.Count;
        }
    }
}
