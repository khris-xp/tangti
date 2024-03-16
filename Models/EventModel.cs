using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tangti.Models

{
    public class Event
    {
        public class StartEndDate
        {
            [BsonElement("start_date")]
            [BsonRepresentation(BsonType.DateTime)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime StartDate { get; set; }

            [BsonElement("end_date")]
            [BsonRepresentation(BsonType.DateTime)]
            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime EndDate { get; set; }
        }


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public required string Title { get; set; }

        [BsonElement("Description")]
        public required string Description { get; set; }

        [BsonElement("Image")]
        public required string Image { get; set; }

        [BsonElement("Category")]
        public required string Category { get; set; }

        [BsonElement("EnrollLimit")]
        public required int EnrollLimit { get; set; }


        [BsonElement("EnrollDate")]
        public required StartEndDate EnrollDate { get; set; }

        [BsonElement("EventDate")]
        public required StartEndDate EventDate { get; set; }

        [BsonElement("CreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; }

        [BsonElement("UpdatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; }


        [BsonElement("Status")]
        public string Status { get; set; } = "Active";

        [BsonElement("CreatedBy")]
        public string? CreatedBy { get; set; }

        [BsonElement("Type")]
        //ต่อคิว
        //เต็มแล้ว enroll ไม่ได้้
        public required string Type { get; set; }


        public Event()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
            CreatedBy = null;
        }
		
		public int members { get; set;}
    }
}
