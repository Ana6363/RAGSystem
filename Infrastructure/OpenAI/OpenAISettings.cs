namespace Infrastructure.OpenAI
{
    public class OpenAISettings
    {
        public string ApiKey { get; set; } = null!;
        public string Model { get; set; } = "gpt-4o";
        public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
    }
}
