using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMDBClone.API.Models
{
    public class CastMember
    {
        [BsonElement("actorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ActorId { get; set; } = null!;

        [BsonElement("character")]
        public string? Character { get; set; }

        [BsonElement("order")]
        public int Order { get; set; } = 0;
    }
}
