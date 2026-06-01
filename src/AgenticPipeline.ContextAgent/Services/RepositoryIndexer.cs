using AgenticPipeline.Infrastructure.Persistence;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pgvector;

namespace AgenticPipeline.ContextAgent.Services;

public sealed class RepositoryIndexer(VectorDbContext dbContext, EmbeddingService embeddingService)
{
    public async Task IndexAsync(string repoPath, CancellationToken ct = default)
    {
        var files = Directory.GetFiles(repoPath, "*.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();
            var content = await File.ReadAllTextAsync(file, ct);
            var tree = CSharpSyntaxTree.ParseText(content);
            var root = await tree.GetRootAsync(ct);
            var types = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();

            foreach (var typeNode in types)
            {
                var symbolName = typeNode.Identifier.ValueText;
                var symbolContent = typeNode.ToFullString();
                var embedding = await embeddingService.GenerateAsync(symbolContent, ct);

                dbContext.CodeChunks.Add(new CodeChunkEntity
                {
                    Id = Guid.NewGuid(),
                    FilePath = file,
                    SymbolName = symbolName,
                    Content = symbolContent,
                    ChunkType = typeNode.Kind().ToString(),
                    Embedding = new Vector(embedding)
                });
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
