using Qdrant.Client;
using Microsoft.Extensions.Options;

namespace Infrastructure.Qdrant
{
    public class QdrantClientProvider
    {
        public QdrantClient Client { get; }

        public QdrantClientProvider(IOptions<QdrantSettings> settings)
        {
            var s = settings.Value;
            Client = new QdrantClient(
                host: s.Host,
                https: true,
                apiKey: s.ApiKey
            );
        }
    }
}
