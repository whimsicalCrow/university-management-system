# Developer Guide

## CQRS conventions
- Commands live under `University.Application/<Feature>/Commands/<Action>` and implement `IRequest<TResponse>`.
- Queries live under `University.Application/<Feature>/Queries/<Action>` and must be read-only.
- Validators use FluentValidation and share the command or query namespace to keep discoverability high.
- Handlers are thin: orchestrate domain services and return DTOs declared in the application layer.

## Pipeline behaviors
- Registered through `AddApplicationLayer` in `University.Application/DependencyInjection/ApplicationServiceCollectionExtensions.cs`.
- `LoggingBehavior<TRequest,TResponse>` logs request name, correlation ID (via `Activity.Current?.Id`) and execution time.
- `ValidationBehavior<TRequest,TResponse>` runs all validators and surfaces FluentValidation errors upstream.
- Add new cross-cutting stages by registering additional `IPipelineBehavior<,>` implementations.

## Sample handlers
- Command sample: `University.Application/Samples/Commands/CreateSampleNote/CreateSampleNoteCommand.cs` shows command, handler, and validator in one file.
- Query sample: `University.Application/Samples/Queries/GetSampleNotes/GetSampleNotesQuery.cs` returns read-only DTOs defined in `University.Application/Samples/Shared/SampleNoteDto.cs`.
- Use the samples as templates when scaffolding new features.

## Naming standards
- Use imperative verb phrases for commands (e.g., `CreateSampleNoteCommand`).
- Use noun or questioning phrases for queries (e.g., `GetSampleNotesQuery`).
- DTOs end in `Dto`; enums or status helpers end in `Statuses`.
- Tests mirror namespaces; see `tests/University.UnitTests/LoggingBehaviorTests.cs` for behavior coverage examples.