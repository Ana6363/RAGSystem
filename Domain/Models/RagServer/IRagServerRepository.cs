namespace Domain.Models.RagServer
{
    public interface IRagServerRepository
    {
        Task<string> QueryAsync(List<string> fileIds, string question);
    }
}
