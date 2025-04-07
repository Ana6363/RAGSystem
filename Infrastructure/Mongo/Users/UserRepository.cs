using Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mongo.Users
{
    class UserRepository : IUserRepository
    {
        private readonly MongoDbContext<User> _users;
        public UserRepository(MongoDbContext database)
        {
            _users = database.GetCollection<User>("Users");
        }
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User?> GetUserByVATNumberAsync(string vatNumber)
        {
            return await _users.Find(user => user.VATNumber == vatNumber).FirstOrDefaultAsync();
        }
        public async Task AddUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }
        public async Task UpdateUserAsync(User user)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }
        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(user => user.Id == id);
        }
    }
}
