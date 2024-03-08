using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models
{
    public class Enroll
    {
        public class JoinUserData
        {
            [BsonElement("UserID")]
            public string UserID { get; set; }
            public DateTime join_date { get; set; }
            public bool enroll_status { get; set; }
            public JoinUserData(string id, bool status)
            {
                UserID = id;
                join_date = DateTime.UtcNow;
                enroll_status = status;
            }
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("EventID")]
        public string EventID { get; set; }

        [BsonElement("Member")]
        public int Member { get; set; }

        [BsonElement("MemberList")]
        public List<JoinUserData> MemberList { get; set; }

        public Enroll()
        {
            MemberList = new List<JoinUserData>();
            Member = MemberList.Count;
            EventID = string.Empty;
        }
    }
}
