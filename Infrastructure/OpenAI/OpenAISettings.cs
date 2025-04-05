namespace Infrastructure.OpenAI
{
    public class OpenAISettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-4o";
        public string ProjectId { get; set; } = string.Empty;
    }
}
