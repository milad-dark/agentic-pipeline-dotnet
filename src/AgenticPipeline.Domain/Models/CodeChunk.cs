namespace AgenticPipeline.Domain.Models;

public sealed class CodeChunk
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string SymbolName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ChunkType { get; set; } = string.Empty;
}
