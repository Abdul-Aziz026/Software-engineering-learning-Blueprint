using Application.Common.Interfaces.Services;
using Domain.Enums;
using Infrastructure.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Llm;

public class LlmFactory : CachingProviderFactory<IChatClient>, ILlmFactory
{
    private readonly GeminiOptions _geminiOptions;
    private readonly ClaudeOptions _claudeOptions;
    private readonly ILoggerFactory _loggerFactory;

    public LlmFactory(
        IOptions<GeminiOptions> geminiOptions,
        IOptions<ClaudeOptions> claudeOptions,
        ILoggerFactory loggerFactory)
    {
        _geminiOptions = geminiOptions.Value;
        _claudeOptions = claudeOptions.Value;
        _loggerFactory = loggerFactory;
    }

    public IChatClient Create(LlmProvider provider) => GetOrCreate(provider);

    protected override IChatClient Build(LlmProvider provider) => provider switch
    {
        LlmProvider.Gemini => GeminiChatClient.CreateGeminiChatClient(_geminiOptions, _loggerFactory),
        LlmProvider.Claude => ClaudeChatClient.CreateClaudeChatClient(_claudeOptions, _loggerFactory),
        _ => throw new NotSupportedException($"LLM provider '{provider}' is not supported.")
    };
}
