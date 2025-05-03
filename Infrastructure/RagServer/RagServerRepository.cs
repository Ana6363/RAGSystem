using Domain.Models.RagServer;
using System.Net.Http.Json;

namespace Infrastructure.RagServer
{
    public class RagServerRepository : IRagServerRepository
    {
        private readonly HttpClient _httpClient;

        public RagServerRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> QueryAsync(List<string> fileIds, string question)
        {
            var payload = new
            {
                file_ids = fileIds,
                question = question
            };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:8000/query", payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<RagResponse>();
            return result?.answer ?? "No answer returned from RAG server.";
        }

        private class RagResponse
        {
            public string answer { get; set; }
        }
    }
}
