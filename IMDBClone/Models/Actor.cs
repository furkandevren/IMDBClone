using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMDBClone.API.Models
{
    public class Actor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("bio")]
        public string? Bio { get; set; }

        [BsonElement("birthDate")]
        public DateTime? BirthDate { get; set; }

        [BsonElement("photoUrl")]
        public string? PhotoUrl { get; set; }

        [BsonElement("knownFor")]
        public List<string> KnownForMovieIds { get; set; } = new();

        [BsonElement("cast")]
        public List<CastMember> Cast { get; set; } = new();

    }
}
