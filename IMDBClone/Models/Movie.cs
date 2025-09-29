using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMDBClone.API.Models
{
    public class Movie
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("overview")]
        public string Overview { get; set; } = null!;

        [BsonElement("genres")]
        public List<string> Genres { get; set; } = new();

        [BsonElement("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [BsonElement("posterUrl")]
        public string? PosterUrl { get; set; }

        [BsonElement("rating")]
        public double Rating { get; set; } = 0;

        [BsonElement("ratingCount")]
        public int RatingCount { get; set; } = 0;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("reviews")]
        public List<string> ReviewIds { get; set; } = new();

        [BsonElement("cast")]
        public List<CastMember> Cast { get; set; } = new();
    }
}
