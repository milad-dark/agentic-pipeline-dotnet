using AgenticPipeline.CodeAgent.Activities;
using AgenticPipeline.CodeAgent.Services;
using AgenticPipeline.ContextAgent.Activities;
using AgenticPipeline.ContextAgent.Services;
using AgenticPipeline.GitHubIntegration.Activities;
using AgenticPipeline.GitHubIntegration.Services;
using AgenticPipeline.Orchestrator.Activities;
using AgenticPipeline.Orchestrator.Workflows;
using AgenticPipeline.Orchestrator.Workers;
using AgenticPipeline.PlanningAgent.Activities;
using AgenticPipeline.PlanningAgent.Services;
using AgenticPipeline.QAAgent.Activities;
using AgenticPipeline.QAAgent.Services;
using AgenticPipeline.ReviewerAgent.Activities;
using AgenticPipeline.ReviewerAgent.Services;
using AgenticPipeline.Sandbox.Services;
using AgenticPipeline.Security.Services;
using Docker.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Octokit;

namespace AgenticPipeline.Orchestrator.Services;

public static class OrchestratorServiceCollectionExtensions
{
    public static IServiceCollection AddAgenticPipelineOrchestrator(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new GitHubClient(new ProductHeaderValue("agentic-pipeline")));
        services.AddSingleton(_ => new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient());
        services.AddSingleton<ITextEmbeddingGenerationService, NoopEmbeddingService>();
        services.AddSingleton<IChatCompletionService, NoopChatCompletionService>();

        services.AddSingleton<InputSanitizer>();
        services.AddSingleton<SecretScanner>();

        services.AddScoped<JiraService>();
        services.AddScoped<PatchApplier>();
        services.AddScoped<ArchitectureGuard>();
        services.AddScoped<TestResultParser>();
        services.AddScoped<EmbeddingService>();
        services.AddScoped<RepositoryIndexer>();
        services.AddScoped<SandboxRunner>();
        services.AddScoped<GitHubAppAuthService>();

        services.AddScoped<SecurityActivities>();
        services.AddScoped<PlanningActivities>();
        services.AddScoped<ContextActivities>();
        services.AddScoped<CodeActivities>();
        services.AddScoped<QAActivities>();
        services.AddScoped<ReviewerActivities>();
        services.AddScoped<GitHubActivities>();
        services.AddScoped<JiraToMergeRequestWorkflow>();

        services.AddSingleton<IPipelineOrchestratorService, PipelineOrchestratorService>();
        services.AddHostedService<TemporalWorkerService>();

        services.AddScoped(_ =>
        {
            var builder = Kernel.CreateBuilder();
            var endpoint = configuration["AzureOpenAI:Endpoint"] ?? "";
            var key = configuration["AzureOpenAI:Key"] ?? "";
            var chatModel = configuration["AzureOpenAI:ChatModel"] ?? "gpt-4o";
            var embeddingModel = configuration["AzureOpenAI:EmbeddingModel"] ?? "text-embedding-3-large";

            if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(key))
            {
                builder.AddAzureOpenAIChatCompletion(chatModel, endpoint, key);
                builder.AddAzureOpenAITextEmbeddingGeneration(embeddingModel, endpoint, key);
            }
            else
            {
                builder.Services.AddSingleton<IChatCompletionService, NoopChatCompletionService>();
                builder.Services.AddSingleton<ITextEmbeddingGenerationService, NoopEmbeddingService>();
            }

            return builder.Build();
        });

        return services;
    }

    private sealed class NoopChatCompletionService : IChatCompletionService
    {
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public Task<ChatMessageContent> GetChatMessageContentAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default) => Task.FromResult(new ChatMessageContent(AuthorRole.Assistant, "{}"));

        public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ChatMessageContent>>([new ChatMessageContent(AuthorRole.Assistant, "{}")]);

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return new StreamingChatMessageContent(AuthorRole.Assistant, "{}");
        }
    }

    private sealed class NoopEmbeddingService : ITextEmbeddingGenerationService
    {
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

        public Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
            IList<string> data,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IList<ReadOnlyMemory<float>>>(data.Select(_ => new ReadOnlyMemory<float>([0.1f, 0.2f, 0.3f])).ToList());
    }
}
