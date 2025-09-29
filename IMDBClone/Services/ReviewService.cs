using IMDBClone.API.Dtos;
using IMDBClone.API.Dtos.ReviewDtos;
using IMDBClone.API.Models;
using IMDBClone.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IMDBClone.API.Services
{
    public class ReviewService
    {
        private readonly IMongoCollection<Review> _reviews;

        public ReviewService(IOptions<MongoDbSettings> settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _reviews = database.GetCollection<Review>("Reviews");
        }

        public async Task<List<Review>> GetByMovieIdAsync(string movieId)
        {
            return await _reviews.Find(r => r.MovieId == movieId).ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(string id)
        {
            return await _reviews.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Review> CreateAsync(ReviewCreateDto dto, string userId)
        {
            var review = new Review
            {
                Content = dto.Content,
                Rating = dto.Rating,
                MovieId = dto.MovieId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _reviews.InsertOneAsync(review);
            return review;
        }

        public async Task<bool> UpdateAsync(ReviewUpdateDto dto, string userId)
        {
            var review = await GetByIdAsync(dto.Id);
            if (review == null || review.UserId != userId) return false;

            var update = Builders<Review>.Update
                .Set(r => r.Content, dto.Content)
                .Set(r => r.Rating, dto.Rating);

            var result = await _reviews.UpdateOneAsync(r => r.Id == dto.Id, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var review = await GetByIdAsync(id);
            if (review == null || review.UserId != userId) return false;

            var result = await _reviews.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
