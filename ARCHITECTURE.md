# Architecture

## Overview

NetDashboard is a Windows desktop application built with WPF and .NET 8,
using MVVM architecture for clean separation of UI and logic.

```
NetDashboard/
├── App.xaml                # Application entry
├── MainWindow.xaml         # Shell / navigation
├── ViewModels/
│   ├── NetworkViewModel.cs
│   └── MailViewModel.cs
├── Views/
│   ├── NetworkView.xaml
│   └── MailView.xaml
├── Services/
│   ├── PingService.cs
│   ├── PortScanService.cs
│   └── MailTestService.cs
├── Localization/
│   ├── Strings.de.resx
│   ├── Strings.en.resx
│   └── Strings.fr.resx
└── Themes/
    └── Dark.xaml
```

## Design Decisions

- **WPF + MVVM:** Standard Windows desktop pattern, testable ViewModels.
- **Dark theme:** Custom resource dictionary for consistent dark UI.
- **Multi-language:** RESX-based localization, switchable at runtime.

## CI

```yaml
name: CI
on: [push, pull_request]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: {dotnet-version: '8.0.x'}
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build
```
