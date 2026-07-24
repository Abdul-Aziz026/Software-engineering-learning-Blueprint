using GenerativeAI.Microsoft;
using Infrastructure.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Llm;

/// <summary>
/// Builds the Gemini embedding pipeline (mirrors <see cref="GeminiChatClient"/>): construct the
/// provider generator, wrap it in <see cref="ResilientEmbeddingGenerator"/>, and hand back the
/// interface. Kept out of the factory so the factory stays a pure provider switch.
/// </summary>
public class GeminiEmbeddingGenerator
{
    public static IEmbeddingGenerator<string, Embedding<float>> CreateGeminiEmbeddingGenerator(
        GeminiOptions geminiOptions, ILoggerFactory loggerFactory)
    {
        // SDK's embedding generator, ctor mirrors the chat client's (apiKey, model).
        var inner = new GenerativeAIEmbeddingGenerator(
            geminiOptions.ApiKey, geminiOptions.EmbeddingModel);

        return inner
            .AsBuilder()
            .Use(g => new ResilientEmbeddingGenerator(
                g, loggerFactory.CreateLogger<ResilientEmbeddingGenerator>()))
            .Build();
    }
}
