using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Domain.Models;
using Domain.Models.Users;
using Domain.Models.ChatHistories;

namespace Infrastructure.Mongo
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

        public IMongoCollection<Document> Documents =>_database.GetCollection<Document>("Documents");
        
        public IMongoCollection<ChatHistory> ChatHistories=>_database.GetCollection<ChatHistory>("ChatHistories");



    }
}
