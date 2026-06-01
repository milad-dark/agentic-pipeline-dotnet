using Microsoft.SemanticKernel.Embeddings;

namespace AgenticPipeline.ContextAgent.Services;

public sealed class EmbeddingService(ITextEmbeddingGenerationService embeddingService)
{
    public async Task<float[]> GenerateAsync(string text, CancellationToken ct = default)
    {
        var embedding = await embeddingService.GenerateEmbeddingAsync(text, cancellationToken: ct);
        return embedding.ToArray();
    }
}
