using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.Http;


namespace Infrastructure.OpenAI
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAISettings _settings;

        public OpenAIService(IHttpClientFactory httpClientFactory, IOptions<OpenAISettings> settings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _settings = settings.Value;

            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }

        public async Task<string> AskChatAsync(string userPrompt)
        {
            var payload = new
            {
                model = _settings.Model,
                messages = new[]
                {
                    new { role = "user", content = userPrompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()!;
        }
    }
}
