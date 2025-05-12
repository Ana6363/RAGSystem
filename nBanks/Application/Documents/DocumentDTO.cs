namespace nBanks.Application.Documents
{
    public class DocumentDTO
    {
        public string? Id { get; set; }

        public string UserId { get; set; }

        public string FileName { get; set; }

        public string Content { get; set; }
        public byte[]? FileData { get; set; }

        public DocumentDTO(string userId, string fileName, string content, string? id = null, byte[]? fileData = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentNullException(nameof(content), "Document content cannot be empty.");

            UserId = userId;
            FileName = fileName;
            Content = content;
            Id = id;
            FileData = fileData;
        }
    }
}
