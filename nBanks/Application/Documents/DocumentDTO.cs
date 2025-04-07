namespace nBanks.Application.Documents
{
    public class DocumentDTO
    {
        public string? Id { get; set; }

        public string UserId { get; set; }

        public string FileName { get; set; }

        public string Content { get; set; }

        public DocumentDTO(string content, string fileName, string userId, string? id = null)
        {
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException(nameof(content));

            FileName = fileName;
            Content = content;
            Id = id;
            UserId = userId;
        }
    }
}
