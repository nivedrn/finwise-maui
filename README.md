# FinWise
A .NET MAUI application for managing expenses, budget and money owed between people

## Projects

| Project | What it contains |
| --- | --- |
| `finwise.maui` | The MAUI app (pages, view models, navigation). |
| `FinWise.Core` | Domain model (`Transaction`, `Person`, `Category`, `Split`, `Settlement`), split and balance calculations, validation and repository interfaces. Plain `net10.0`, no dependencies. |
| `FinWise.Data` | SQLite storage (sqlite-net-pcl) implementing the repository interfaces, with versioned schema migrations and seeded default categories. |
| `FinWise.Tests` | xUnit tests for Core and Data. The Data tests run against temporary SQLite files. |

## Building

Requires the .NET 10 SDK and the MAUI workload.

```sh
dotnet workload install maui          # or maui-android on Linux
dotnet build finwise.maui -f net10.0-android
```

iOS and Mac Catalyst builds need a Mac with Xcode; the Windows target is only added when building on Windows.

## Tests

The core logic and storage don't depend on MAUI, so the tests run anywhere the .NET 10 SDK is installed (no workload needed):

```sh
dotnet test FinWise.Tests
```

Building the whole `FinWise.sln` also builds the app for every platform, which needs the matching workloads (and a Mac for iOS).
