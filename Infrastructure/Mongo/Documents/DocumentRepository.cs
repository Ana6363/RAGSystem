using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.Documents;
using MongoDB.Driver;

namespace Infrastructure.Mongo.Documents
{
    public class DocumentRepository : IDocumentRepository
    {

        private readonly IMongoCollection<Document> _documents;
        public DocumentRepository() { }

        public DocumentRepository(MongoDbContext database)
        {
            _documents = database.Documents;
        }

        public async Task<Document?> GetDocumentByNameAsync(string name)
        {
            return await _documents.Find(document => document.fileName.FileNameValue == name).FirstOrDefaultAsync();
        }

        public async Task<List<Document>?> GetDocumentByUserIdAsync(string userId)
        {
            return await _documents.Find(document => document.UserId == userId).ToListAsync();
        }


        public async Task AddDocumentAsync(Document document)
        {
            await _documents.InsertOneAsync(document);
        }

        public async Task DeleteDocumentAsync(string id)
        {
            await _documents.DeleteOneAsync(document => document.Id == id);
        }


    }
}
