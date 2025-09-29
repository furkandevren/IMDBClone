using IMDBClone.API.Dtos.MovieDtos;
using IMDBClone.API.Dtos.ReviewDtos;
using IMDBClone.API.Models;
using IMDBClone.API.Settings;
using MongoDB.Driver;

namespace IMDBClone.API.Services
{
    public class MovieService
    {
        private readonly IMongoCollection<Movie> _movies;

        public MovieService(IMongoClient client, IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var database = client.GetDatabase(settings.DatabaseName);
            _movies = database.GetCollection<Movie>("Movies");
        }

        public async Task<List<Movie>> GetAllAsync(int page = 1, int pageSize = 20, string? genre = null)
        {
            var filter = Builders<Movie>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(genre))
                filter = Builders<Movie>.Filter.AnyEq(m => m.Genres, genre);

            return await _movies.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();


        }
        public async Task<List<Movie>> GetAllAsync() =>
        await _movies.Find(_ => true).ToListAsync();

        public async Task<Movie> GetByIdAsync(string id) =>
            await _movies.Find(m => m.Id == id).FirstOrDefaultAsync();

        public async Task<Movie> CreateAsync(Movie movie)
        {
            await _movies.InsertOneAsync(movie);
            return movie;
        }

        public async Task<bool> UpdateAsync(string id, Movie updatedMovie)
        {
            var result = await _movies.ReplaceOneAsync(m => m.Id == id, updatedMovie);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _movies.DeleteOneAsync(m => m.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<MovieWithReviewsDto?> GetWithReviewsAsync(string id, ReviewService reviewService)
        {
            var movie = await GetByIdAsync(id);
            if (movie == null) return null;

            var reviews = await reviewService.GetByMovieIdAsync(movie.Id);

            return new MovieWithReviewsDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Genres = movie.Genres, // List<string>
                Year = movie.Year,     // ReleaseDate yerine Year kullanıyoruz
                Overview = movie.Overview,
                PosterUrl = movie.PosterUrl,
                Rating = movie.Rating,
                RatingCount = movie.RatingCount,
                CreatedAt = movie.CreatedAt,
                Reviews = reviews.Select(r => new ReviewReadDto
                {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating,
                    UserId = r.UserId,
                    MovieId = r.MovieId,
                    CreatedAt = r.CreatedAt
                }).ToList()
            };
        }

    }
}
