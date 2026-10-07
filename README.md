# FlowTask - Professional Cross-Platform Productivity & Task Manager Desktop Application

> A commercial-grade, high-performance desktop productivity and task management suite built with **.NET MAUI** (.NET 10 & C# 13), optimized for **Windows 11 desktop** and portable to macOS.

---

## 🌟 Key Highlights & Design Inspiration

FlowTask merges modern desktop ergonomics and UI patterns from **Windows 11 Task Manager**, **Linear**, **Notion**, **Microsoft To Do**, **VS Code**, and **Trello**:

* **Windows 11 Fluent Visuals**: Layered acrylic surfaces, rounded borders, soft depth, customizable accent palettes, and instant **Light / Dark / System** theme switching with native `AppThemeBinding`.
* **Collapsible Desktop Shell**: Collapsible sidebar navigation (compact vs. expanded), top toolbar with workspace switcher, global search (Ctrl+K), and notification tray.
* **Full-Featured Task Management**:
  - Task statuses: `Inbox`, `Todo`, `In Progress`, `Blocked`, `Review`, `Done`, `Cancelled`.
  - Five priority levels: `Urgent`, `High`, `Medium`, `Low`, `None`.
  - Due dates, start dates, duration estimates, subtasks, tags, and dependencies.
  - Slide-in inspector panel for rapid editing without losing context.
* **Kanban Board**: Multi-column board (`Inbox`, `Todo`, `In Progress`, `Blocked`, `Review`, `Done`) with WIP counters and quick task capture.
* **Interactive Calendar & Gantt Timeline**: Day, Week, and Month calendar scheduling + date-range timeline visualization with today marker.
* **Resource Center & Workload Monitor**:
  - Real-time CPU-style workload meter measuring task capacity.
  - Active, Blocked, Overdue, and Due Today KPI stat badges.
  - Project workload allocation bars.
  - GitHub-style 52-week productivity activity heatmap.
* **Distraction-Free Focus Mode (Pomodoro)**: Circular progress timer with 25-minute Pomodoro cycles, session counting, pause/resume, and event aggregator triggers.
* **Integrated Time Tracking**: One-click active timer tracking, daily task duration rollups, and manual time entries.
* **SQLite Persistence with EF Core**: Fully configured SQLite database with automated migration/creation, cascading rules, timestamp auditing, and demo dataset seeding.

---

## 🏗 Solution Architecture

```
FlowTask/
├── src/
│   ├── TaskManager.Core/           # Domain Layer (Zero external dependencies)
│   │   ├── Entities/               # TaskItem, Project, Workspace, Tag, TimeEntry, FocusSession, etc.
│   │   ├── Enums/                  # TaskItemStatus, TaskPriority, RecurrenceType, etc.
│   │   ├── Interfaces/             # Service & Repository contracts (ITaskService, ITaskRepository...)
│   │   ├── DTOs/                   # DashboardStatsDto, ProjectStatsDto, SearchResultDto, etc.
│   │   ├── Events/                 # Domain events & Pub/Sub EventAggregator
│   │   └── Constants/              # AppConstants, ThemeConstants
│   │
│   ├── TaskManager.Application/    # Business Logic Layer (Clean CQRS/Service pattern)
│   │   ├── Services/               # TaskService, ProjectService, AnalyticsService, FocusService...
│   │   ├── ViewModels/             # CommunityToolkit.Mvvm ViewModels (Dashboard, Kanban, Focus...)
│   │   ├── Validators/             # FluentValidation rules (TaskValidator, ProjectValidator...)
│   │   └── DependencyInjection/    # AddApplicationServices() DI module
│   │
│   ├── TaskManager.Infrastructure/ # Persistence & I/O Layer
│   │   ├── Database/               # FlowTaskDbContext (Fluent API, SQLite DateTimeOffset conversions)
│   │   ├── DatabaseInitializer.cs  # Schema creation & 40+ demo tasks, projects, tags, widgets seeding
│   │   ├── Repositories/           # Generic Repository<T> & specialized Entity repositories
│   │   ├── Services/               # SettingsService, DataService (JSON export/import/backup)
│   │   └── DependencyInjection/    # AddInfrastructure() DI module
│   │
│   ├── TaskManager.App/            # Presentation Layer (.NET MAUI)
│   │   ├── Views/                  # DashboardView, TaskListView, KanbanView, CalendarView, etc.
│   │   ├── Converters/             # BoolToVisibility, StatusToColor, PriorityToColor, etc.
│   │   ├── Resources/              # Fluent styles, colors (Light/Dark), typography
│   │   ├── MainPage.xaml           # Desktop shell layout (collapsible sidebar, responsive view host)
│   │   └── MauiProgram.cs          # Dependency injection bootstrap & database initialization
│   │
│   └── TaskManager.Tests/          # Unit & Integration Test Suite
│       ├── TaskServiceTests.cs     # Task CRUD, status workflow & completion tests
│       ├── FocusServiceTests.cs    # Pomodoro session lifecycle & event dispatch tests
│       ├── ProjectServiceTests.cs  # Project statistics & progress calculations
│       ├── AnalyticsServiceTests.cs# Dashboard metrics & workload calculations
│       ├── TimeTrackingServiceTests# Active timer exclusivity & duration aggregation
│       ├── SearchServiceTests.cs   # Multi-entity search & filtering tests
│       ├── ValidationTests.cs      # FluentValidation boundary tests
│       └── DatabaseIntegrationTests# Real EF Core SQLite seeding & database CRUD tests
```

---

## 🛠 Technology Stack

| Technology | Purpose |
|---|---|
| **.NET 10** | High-performance modern managed runtime |
| **.NET MAUI** | Cross-platform native UI framework (Windows App SDK / WinUI 3) |
| **C# 13** | Primary language with modern pattern matching and primary constructors |
| **CommunityToolkit.Mvvm** | Source-generated MVVM observable properties & commands |
| **Entity Framework Core 10** | Code-first ORM with Fluent API schema definition |
| **SQLite (SQLitePCLRaw)** | Embedded local transactional database storage |
| **FluentValidation** | Validating user inputs for tasks, projects, and time entries |
| **System.Text.Json** | Fast JSON serialization for export, import, and settings |
| **xUnit + FluentAssertions + Moq** | Unit and database integration testing |

---

## 🚀 Getting Started

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) with .NET MAUI workload:
  ```bash
  dotnet workload install maui
  ```
* Windows 10/11 (version 1809 or higher) or macOS (15.0+)

### Building the Solution

Clone or navigate to the repository directory and run:

```bash
cd src
dotnet build FlowTask.slnx
```

To build specifically for Windows desktop:

```bash
cd src
dotnet build TaskManager.App -f net10.0-windows10.0.19041.0
```

### Running Tests

Execute all 21 unit and database integration tests:

```bash
cd src
dotnet test TaskManager.Tests
```

### Running the Application

To launch FlowTask on Windows desktop:

```bash
cd src
dotnet run --project TaskManager.App -f net10.0-windows10.0.19041.0
```

*On first launch, FlowTask automatically creates the SQLite database at `%LOCALAPPDATA%\FlowTask\flowtask.db` and seeds comprehensive demo data including 2 workspaces, 5 projects, 10 tags, 40+ tasks across all statuses, time logs, focus sessions, and dashboard widgets.*

---

## ⌨️ Desktop Keyboard Shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl + K` | Toggle Global Search Modal |
| `Ctrl + N` | Create New Task |
| `Ctrl + Shift + F` | Switch to Focus Mode |
| `Esc` | Close Search Modal / Close Task Details Panel |

---

## 📄 License
MIT License. Built for modern desktop productivity.
