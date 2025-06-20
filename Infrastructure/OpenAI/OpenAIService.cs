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

        public virtual async Task<string> AskChatAsync(string userPrompt)
        {
            try
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
                request.Headers.Add("OpenAI-Project", _settings.ProjectId);

                var payload = new
                {
                    model = _settings.Model,
                    messages = new[]
                    {
                        new { role = "user", content = userPrompt }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GenerateChatTitleAsync(string initialContent)
        {
            var prompt = $"Generate a concise, meaningful title for this chat based on:\n\n{initialContent}";

            var response = await AskChatAsync(prompt);

            var title = response?.Trim() ?? "New Chat";

            return title;
        }

    }
}
