using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMDBClone.API.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("UserId")]
        public string UserId { get; set; } = string.Empty;  // Yorumu kimin yazdığı

        [BsonElement("MovieId")]
        public string MovieId { get; set; } = string.Empty; // Hangi filme ait

        [BsonElement("Comment")]
        public string Comment { get; set; } = string.Empty;

        [BsonElement("Rating")]
        public int Rating { get; set; } // 1–10 arası puan

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
