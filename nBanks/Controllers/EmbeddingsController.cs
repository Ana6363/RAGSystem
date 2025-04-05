using Microsoft.AspNetCore.Mvc;
using Qdrant.Client;
using Infrastructure.Qdrant;


[ApiController]
[Route("api/[controller]")]
public class EmbeddingsController : ControllerBase
{
    private readonly QdrantClient _qdrant;

    public EmbeddingsController(QdrantClientProvider provider)
    {
        _qdrant = provider.Client;
    }

    [HttpGet("collections")]
    public async Task<IActionResult> ListCollections()
    {
        var collections = await _qdrant.ListCollectionsAsync();
        return Ok(collections);
    }


    // FUTURE: insert vector, search, delete, etc.
}
