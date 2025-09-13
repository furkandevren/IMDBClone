using IMDBClone.API.Models;
using IMDBClone.API.Settings;
using MongoDB.Driver;

namespace IMDBClone.API.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoClient client, IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>()!;
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task<User?> GetByIdAsync(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User?> ValidateCredentialsAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null) return null;

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;

            return null; 
        }


    }
}
