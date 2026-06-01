# AgenticPipeline (.NET 8)

Enterprise agentic pipeline that automates Jira task analysis to draft GitHub PR creation through orchestrated specialist agents.

## Architecture

```text
Jira Input -> Security -> Planning -> Context (RAG) -> Code -> QA Sandbox -> Reviewer -> GitHub Draft PR
                              ^                                              |
                              +-------------- Temporal Orchestrator ----------+
```

## Prerequisites
- .NET 8 SDK
- Docker / Docker Compose
- PostgreSQL (pgvector)
- Redis
- RabbitMQ
- Temporal

## Quick Start
1. `docker compose up -d`
2. `dotnet restore AgenticPipeline.sln`
3. `dotnet build AgenticPipeline.sln`
4. `dotnet run --project src/AgenticPipeline.API/AgenticPipeline.API.csproj`

## Configuration
All runtime settings are in `/src/AgenticPipeline.API/appsettings.json`.

## Project Structure
| Project | Responsibility |
|---|---|
| AgenticPipeline.API | HTTP API and webhook surface |
| AgenticPipeline.Orchestrator | Workflow orchestration |
| AgenticPipeline.PlanningAgent | Jira task decomposition |
| AgenticPipeline.ContextAgent | Repository indexing and context retrieval |
| AgenticPipeline.CodeAgent | Patch generation and application |
| AgenticPipeline.ReviewerAgent | Architecture and AI review |
| AgenticPipeline.QAAgent | TRX parsing and QA execution |
| AgenticPipeline.GitHubIntegration | GitHub auth and PR operations |
| AgenticPipeline.Sandbox | Isolated test execution |
| AgenticPipeline.Security | Prompt injection and secret checks |
| AgenticPipeline.Infrastructure | EF Core, cache, messaging |
| AgenticPipeline.Domain | Core contracts and models |

## Agent Responsibilities
- PlanningAgent: never generates code, only JSON plans.
- ContextAgent: builds relevant context from repository chunks.
- CodeAgent: emits unified diff patches only.
- ReviewerAgent: checks architecture/security/async boundaries.
- QAAgent: runs and parses test results.

## Technology Stack
ASP.NET Core, Temporal, Semantic Kernel, PostgreSQL + pgvector, Redis, RabbitMQ, Docker, Octokit.

## Contributing
- Keep patch-based edits.
- Respect architecture boundaries.
- Use async/await (no `.Result`/`.Wait()`).
