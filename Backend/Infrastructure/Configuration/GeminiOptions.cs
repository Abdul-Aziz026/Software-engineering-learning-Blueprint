
namespace Infrastructure.Configuration;

public class GeminiOptions
{
    public string ApiKey { get; set; }

    /// <summary>Chat/completion model id (e.g. "gemini-2.0-flash").</summary>
    public string Model { get; set; }

    /// <summary>
    /// Embedding model id, separate from <see cref="Model"/>. Vectors are only comparable within the
    /// same model, so store this id with every vector — changing it breaks an existing vector store.
    /// </summary>
    public string EmbeddingModel { get; set; } = "text-embedding-004";
}
