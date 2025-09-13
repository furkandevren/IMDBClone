using IMDBClone.API.Models;
using IMDBClone.API.Settings;
using MongoDB.Driver;

namespace IMDBClone.API.Services
{
    public class ReviewService
    {
        private readonly IMongoCollection<Review> _reviews;

        public ReviewService(IMongoClient client, IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var database = client.GetDatabase(settings.DatabaseName);
            _reviews = database.GetCollection<Review>("Reviews");
        }

        public async Task<List<Review>> GetByMovieIdAsync(string movieId) =>
            await _reviews.Find(r => r.MovieId == movieId).ToListAsync();

        public async Task CreateAsync(Review review) =>
            await _reviews.InsertOneAsync(review);
    }
}
