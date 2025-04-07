using Domain.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext database)
        {
            _users = database.Users;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.UserId.Value == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByVATNumberAsync(string vatNumber)
        {
            return await _users.Find(user => user.VATNumber.ToString() == vatNumber).FirstOrDefaultAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }


        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(user => user.UserId.Value == id);
        }

    }
}
