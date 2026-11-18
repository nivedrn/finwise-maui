# FinWise
A .NET MAUI application for managing expenses, budget and money owed between people

## Building

Requires the .NET 10 SDK and the MAUI workload.

```sh
dotnet workload install maui          # or maui-android on Linux
dotnet build finwise.maui -f net10.0-android
```

iOS and Mac Catalyst builds need a Mac with Xcode; the Windows target is only added when building on Windows.
