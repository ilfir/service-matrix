# Service Matrix Agent Guidelines

These guidelines are for any AI coding agent helping maintain and improve the Service Matrix project.

Service Matrix is an ASP.NET Core Web API using .NET 10.0. It provides word search functionality over a letter matrix, backed by file-based dictionary data. The project uses a command/query pattern, controller endpoints under the API project, helper classes for file and word-search logic, and xUnit tests under `Tests/service-matrix-tests`.

## Working Rules

1. Read the relevant project files before making changes.
2. Keep changes focused on the requested task.
3. Preserve existing API behavior unless a change is explicitly requested.
4. Update or add tests when behavior changes.
5. Run the relevant tests before considering the work complete.
6. No hallucinations allowed. Only state facts that are supported by the repository, command output, or clearly labeled assumptions.
7. Ask questions when something is not clear.
8. Do not overwrite, revert, delete, or reformat existing user changes unless explicitly asked.
9. If unrelated local changes are present, leave them alone.
10. When making claims about the codebase, cite the relevant file, command output, or test result.
11. If a request has multiple reasonable interpretations, ask one concise clarifying question before making a risky change.
12. If an assumption is low-risk, state the assumption and continue.
13. Prefer small, testable changes.
14. Preserve public API contracts, response shapes, and existing tests unless the user explicitly requests a breaking change.
15. Keep controllers thin; put business logic in handlers, helpers, or services.
16. Use existing .NET patterns in the repository before introducing new abstractions.
17. At the end of work, summarize changed files, tests run, and any remaining risks or follow-up items.

## Definition of Done

A task is complete only when:

- The requested change is implemented.
- Relevant build or test checks have been run.
- Any failed, skipped, or unavailable checks are reported clearly.
- Remaining assumptions, risks, or follow-up items are called out.

## Project Notes

- Main solution file: `service-matrix.sln`
- API project: `API/service-matrix.csproj`
- Test project: `Tests/service-matrix-tests/service-matrix-tests.csproj`
- Main controller: `API/Controllers/WordSearchController.cs`
- Core word search logic: `API/Helpers/WordSearchHelper.cs`
- File helper abstraction: `API/Interfaces/IFileHelper.cs`
- File helper implementation: `API/Helpers/FileHelper.cs`
- Project context: `PROJECT_CONTEXT.md`
- Improvement tracking: `improvement_plan.md`

## Development Commands

```bash
dotnet restore
dotnet build service-matrix.sln
dotnet test service-matrix.sln
```

## Expected Behavior

When working in this repository:

- Prefer the existing code style and architecture.
- Avoid unrelated refactors.
- Do not invent project requirements.
- Do not claim tests passed unless they were actually run.
- If a command fails, report the failure and the relevant output.
- If the request is ambiguous, pause and ask a concise clarifying question.
- When changing behavior, update tests in `Tests/service-matrix-tests`.
- When changing API behavior, verify the expected status codes and JSON response shapes.
