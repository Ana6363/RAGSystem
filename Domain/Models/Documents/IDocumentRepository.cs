using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Documents
{
    public interface IDocumentRepository
    {
        Task<Document?> GetDocumentByNameAsync(string name);
        Task<List<Document>?> GetDocumentByUserIdAsync(string userId);
        Task AddDocumentAsync(Document document);
        Task DeleteDocumentAsync(string id);
        Task<List<Document>?> GetDocumentByNameAndUserAsync(string name, string userId);
        Task<List<Document>> GetDocumentsByIdsAsync(List<string> ids);

    }
}
