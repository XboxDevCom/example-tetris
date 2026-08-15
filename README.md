# Example: Tetris

A Universal Windows Platform (UWP) Tetris game for Xbox One, built with C# and XAML.

## Technologies

- **Platform:** Universal Windows Platform (UWP)
- **Target:** Xbox One
- **Language:** C# / XAML
- **IDE:** Visual Studio 2017
- **Framework:** Microsoft.NETCore.UniversalWindowsPlatform 6.2.14

## Getting Started

1. Open `Tetris.sln` in Visual Studio 2017.
2. Select the **x64** configuration.
3. Deploy to your Xbox One (Developer Mode) or the Xbox One simulator.
4. Press **Start** to begin playing.

## Controls

| Input | Action |
|-------|--------|
| D-Pad Left / Left Stick Left | Move piece left |
| D-Pad Right / Left Stick Right | Move piece right |
| D-Pad Down / Left Stick Down | Soft drop |
| D-Pad Up / Left Stick Up | Hard drop |
| A Button / Space | Rotate piece |

## Project Structure

```
Tetris/
├── App.xaml              # Application definition
├── App.xaml.cs           # Application code-behind
├── MainPage.xaml         # Game UI
├── MainPage.xaml.cs      # Game controller and rendering
├── Package.appxmanifest  # Package manifest
├── Models/
│   ├── Tetromino.cs      # Tetromino piece model with rotation
│   └── GameBoard.cs      # Game board logic
├── Properties/
│   ├── AssemblyInfo.cs   # Assembly metadata
│   └── Default.rd.xml    # .NET Native runtime directives
└── Assets/               # App icons and splash screen
```

## License

MIT License — see [LICENSE](LICENSE).
