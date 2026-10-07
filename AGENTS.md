# FlowTask - Architecture & Development Guide

## Overview

**FlowTask** is a professional cross-platform task management and productivity desktop application built with .NET MAUI, primarily optimized for Windows desktop.

## Architecture

```
src/
├── TaskManager.App/         # .NET MAUI UI layer
│   ├── Views/               # XAML views (ContentView-based)
│   ├── Controls/            # Custom reusable controls
│   ├── Converters/          # Value converters
│   ├── Behaviors/           # XAML behaviors
│   ├── Resources/           # Styles, Colors, Fonts, Images
│   └── Platforms/           # Platform-specific code
│
├── TaskManager.Core/        # Domain layer (no dependencies)
│   ├── Entities/            # Domain entities (EF Core models)
│   ├── Enums/               # Status, Priority, etc.
│   ├── Interfaces/          # Service contracts
│   ├── DTOs/                # Data transfer objects
│   ├── Events/              # Domain events + EventAggregator
│   └── Constants/           # App-wide constants
│
├── TaskManager.Application/ # Business logic layer
│   ├── Services/            # Service implementations
│   ├── ViewModels/          # MVVM ViewModels
│   ├── Validators/          # FluentValidation validators
│   └── DependencyInjection/ # DI registration
│
├── TaskManager.Infrastructure/ # Data access layer
│   ├── Database/            # EF Core DbContext, Initializer
│   ├── Repositories/        # Repository implementations
│   ├── Services/            # Infrastructure services
│   └── DependencyInjection/ # DI registration
│
└── TaskManager.Tests/       # Unit & integration tests
```

## Technology Stack

| Technology | Purpose |
|---|---|
| .NET 10 | Runtime |
| .NET MAUI | UI Framework |
| C# 13 | Language |
| CommunityToolkit.Mvvm | MVVM Framework |
| CommunityToolkit.Maui | MAUI Extensions |
| Entity Framework Core | ORM |
| SQLite | Database |
| FluentValidation | Input Validation |
| SkiaSharp | Advanced Visualization |
| xUnit + Moq + FluentAssertions | Testing |

## Architecture Rules

1. **No business logic in XAML code-behind** - Code-behind files should only contain DI constructor injection and minimal UI event handlers
2. **No direct database access from Views** - Always go through ViewModel → Service → Repository
3. **No static service locator** - Use constructor injection via DI
4. **Use dependency injection** - All services registered in DI container
5. **Keep ViewModels testable** - No direct platform dependencies
6. **Use async APIs** - All I/O operations must be async
7. **Avoid blocking the UI thread** - Never use `.Result` or `.Wait()`
8. **Use CancellationToken** - For cancellable operations
9. **Prefer composition over inheritance** - Keep components reusable
10. **Keep UI state separate from domain entities** - Use DTOs where appropriate
11. **Validate all user input** - Use FluentValidation

## Layer Dependencies

```
TaskManager.App → TaskManager.Application → TaskManager.Core
                → TaskManager.Infrastructure → TaskManager.Core
                                             → TaskManager.Application
```

Core has NO external dependencies (except CommunityToolkit.Mvvm for ObservableObject).

## MVVM Pattern

- **ViewModels** use `[ObservableProperty]` attributes from CommunityToolkit.Mvvm
- **Commands** use `[RelayCommand]` and `[RelayCommand(AllowConcurrentExecutions = false)]`
- **Collections** use `ObservableCollection<T>`
- **Navigation** is handled through the MainViewModel page switching pattern

## Database

- SQLite database stored in app data folder
- EF Core with Fluent API configuration
- Auto-seeded demo data on first launch
- Automatic `UpdatedAt` timestamp handling
- DateTimeOffset conversion for SQLite compatibility

## Theming

- Three modes: System, Light, Dark
- Uses `AppThemeBinding` in XAML for automatic theme switching
- Windows 11-inspired color palette
- Theme preference persisted in UserPreference table

## Key Namespaces

- `TaskManager.Core.Entities` - Domain models
- `TaskManager.Core.Enums` - Enumerations
- `TaskManager.Core.Interfaces` - Service contracts
- `TaskManager.Application.Services` - Business logic
- `TaskManager.Application.ViewModels` - MVVM ViewModels
- `TaskManager.Infrastructure.Database` - EF Core context
- `TaskManager.Infrastructure.Repositories` - Data access

## Building

```bash
cd src
dotnet restore
dotnet build
```

## Running (Windows)

```bash
cd src
dotnet run --project TaskManager.App -f net10.0-windows10.0.19041.0
```

## Testing

```bash
cd src
dotnet test TaskManager.Tests
```

## Development Guidelines

### Adding a New Feature

1. Define entities in `TaskManager.Core.Entities`
2. Add service interface in `TaskManager.Core.Interfaces`
3. Create DTOs if needed in `TaskManager.Core.DTOs`
4. Implement service in `TaskManager.Application.Services`
5. Create ViewModel in `TaskManager.Application.ViewModels`
6. Add validator in `TaskManager.Application.Validators`
7. Create View (XAML) in `TaskManager.App.Views`
8. Register in DI container
9. Add navigation entry
10. Write tests

### Adding a New View

1. Create `{Name}View.xaml` as `ContentView` (not `ContentPage`)
2. Create matching `{Name}View.xaml.cs` with DI constructor
3. Bind to ViewModel in code-behind: `BindingContext = viewModel;`
4. Register in DI and navigation switch in MainViewModel

### Database Migrations

Using EF Core migrations:
```bash
cd src/TaskManager.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../TaskManager.App
dotnet ef database update --startup-project ../TaskManager.App
```
