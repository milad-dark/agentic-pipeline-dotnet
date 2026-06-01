using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace AgenticPipeline.Infrastructure.Persistence;

public sealed class VectorDbContext(DbContextOptions<VectorDbContext> options) : DbContext(options)
{
    public DbSet<CodeChunkEntity> CodeChunks => Set<CodeChunkEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.Entity<CodeChunkEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FilePath).IsRequired();
            entity.Property(x => x.SymbolName).IsRequired();
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.ChunkType).IsRequired();
            entity.Property(x => x.Embedding).HasColumnType("vector(1536)");
        });
    }
}

public sealed class CodeChunkEntity
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string SymbolName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ChunkType { get; set; } = string.Empty;
    public Vector Embedding { get; set; } = new(new float[] { 0f });
}
