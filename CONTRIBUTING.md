# Contributing to NetDashboard

## Getting Started

### Prerequisites

- Windows 10 / 11
- Visual Studio 2022+ or JetBrains Rider
- .NET 8.0 SDK
- C# 12

### Setup

1. Fork the repository
2. `git clone https://github.com/YOUR_USERNAME/NetDashboard`
3. Open `NetDashboard.sln` in Visual Studio
4. Build: `dotnet build`
5. Run: `dotnet run --project NetDashboard`

## Development Workflow

1. Create a feature branch: `git checkout -b feature/xyz`
2. Make your changes
3. Run tests: `dotnet test`
4. Commit: `git commit -m "[feat] description"`
5. Push and open a Pull Request

## Code Style

- C#: Follow Microsoft coding conventions
- MVVM pattern throughout WPF
- Format with the built-in Visual Studio formatter

## Commit Convention

`[type] description`, where type is:
- `[feat]`: new feature
- `[fix]`: bug fix
- `[docs]`: documentation only
- `[refactor]`: code cleanup
- `[test]`: tests only

## Security

Found a security issue? See [SECURITY.md](SECURITY.md).

## Questions?

Open an issue or discussion on GitHub.
