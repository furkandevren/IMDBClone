﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IMDBClone.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("username")]
        public string Username { get; set; }= null!;

        [BsonElement("email")]
        public string Email { get; set; }=null!;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;

        [BsonElement("rols")]
        public List<string> Roles { get; set; } = new();

        [BsonElement("ceratedAt")]
        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    }
}
