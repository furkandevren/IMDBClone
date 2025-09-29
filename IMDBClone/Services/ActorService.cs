using IMDBClone.API.Dtos.ActorDtos;
using IMDBClone.API.Models;
using IMDBClone.API.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IMDBClone.API.Services
{
    public class ActorService
    {
        private readonly IMongoCollection<Actor> _actors;
        private readonly IMongoCollection<Movie> _movies;

        public ActorService(IMongoCollection<Actor> actors, IMongoCollection<Movie> movies, IMongoClient client, IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var db = client.GetDatabase(settings.DatabaseName);
            _actors = actors;
            _movies = movies;
        }
        public async Task<List<Actor>> GetAllAsync() =>
           await _actors.Find(_ => true).ToListAsync();

        public async Task<Actor?> GetByIdAsync(string id) =>
            await _actors.Find(a => a.Id == id).FirstOrDefaultAsync();

        public async Task<Actor> CreateAsync(Actor actor)
        {
            await _actors.InsertOneAsync(actor);
            return actor;
        }

        // Filmleri sorgulamak: movie.cast.actorId == actorId
        public async Task<List<Movie>> GetMoviesByActorIdAsync(string actorId)
        {
            var filter = Builders<Movie>.Filter.ElemMatch(m => m.Cast, c => c.ActorId == actorId);
            return await _movies.Find(filter).ToListAsync();
        }

        // Top actors aggregation: group by cast.actorId, avg rating, movieCount
        public async Task<List<TopActorDto>> GetTopActorsAsync(int limit = 10)
        {
            // 1) unwind cast
            var unwind = new BsonDocument("$unwind", "$cast");

            // 2) group by cast.actorId -> compute avg rating and count
            var group = new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$cast.actorId" },
                { "movieCount", new BsonDocument("$sum", 1) },
                { "avgRating", new BsonDocument("$avg", "$rating") },
                { "totalRatingCount", new BsonDocument("$sum", "$ratingCount") }
            });

            // 3) sort by avgRating desc (and movieCount desc)
            var sort = new BsonDocument("$sort", new BsonDocument { { "avgRating", -1 }, { "movieCount", -1 } });

            // 4) limit
            var limitDoc = new BsonDocument("$limit", limit);

            // 5) lookup actor details
            var lookup = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Actors" },
                { "localField", "_id" },
                { "foreignField", "_id" },
                { "as", "actor" }
            });

            var unwindActor = new BsonDocument("$unwind", "$actor");

            // 6) project to desired shape
            var project = new BsonDocument("$project", new BsonDocument
            {
                { "actorId", "$_id" },
                { "name", "$actor.name" },
                { "photoUrl", "$actor.photoUrl" },
                { "avgRating", "$avgRating" },
                { "movieCount", "$movieCount" },
                { "_id", 0 }
            });

            var pipeline = new[] { unwind, group, sort, limitDoc, lookup, unwindActor, project };

            var result = await _movies.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var list = result.Select(doc => new TopActorDto
            {
                ActorId = doc.GetValue("actorId").AsString,
                Name = doc.GetValue("name").AsString,
                PhotoUrl = doc.GetValue("photoUrl").AsNullableString(),
                AvgMovieRating = doc.GetValue("avgRating").ToDouble(),
                MovieCount = doc.GetValue("movieCount").ToInt32()
            }).ToList();

            return list;
        }
    }

    static class BsonExtensions
    {
        public static string? AsNullableString(this BsonValue v) => v.IsBsonNull ? null : v.AsString;
    }

}

