# AIDispatcher

AIDispatcher is an extensible CQRS dispatcher and pipeline system, built to replace MediatR with full control, observability, and performance.

## ✨ Features

- Request/Response handling
- Publish-subscribe notifications
- Pipeline behaviors (Validation, Retry, CircuitBreaker, Logging)
- Pre- and Post-Processors
- Fully DI-based (no reflection or marker interfaces)
- FluentValidation integration
- Observability support (OpenTelemetry ready)

## 📦 Installation

```bash
dotnet add package AIDispatcher
