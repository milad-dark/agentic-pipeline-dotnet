using Testcontainers.PostgreSql;

namespace AgenticPipeline.IntegrationTests;

public sealed class TestContainersSetup : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("pgvector/pgvector:pg16")
        .WithDatabase("agentic_pipeline")
        .WithUsername("pipeline")
        .WithPassword("pipeline_dev")
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync() => await _postgres.StartAsync();

    public async Task DisposeAsync() => await _postgres.DisposeAsync();
}
