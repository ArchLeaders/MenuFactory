# Menu Factory

[![License](https://img.shields.io/badge/License-MIT-blue.svg?logo=github&logoColor=5751ff&labelColor=2A2C33&color=5751ff&style=for-the-badge)](https://github.com/ArchLeaders/MenuFactory/blob/master/License.md) [![Downloads](https://img.shields.io/github/downloads/ArchLeaders/MenuFactory/total?label=downloads&logo=github&logoColor=37c75e&labelColor=2A2C33&color=37c75e&style=for-the-badge)](https://github.com/ArchLeaders/MenuFactory/releases) [![Latest](https://img.shields.io/github/v/tag/ArchLeaders/MenuFactory?label=Release&logo=github&logoColor=324fff&color=324fff&labelColor=2A2C33&style=for-the-badge)](https://github.com/ArchLeaders/MenuFactory/releases/latest)

## Usage

```cs
// Create a MenuFactory
IMenuFactory menuFactory = new AvaloniaMenuFactory();

// Register a menu class
DefaultMenu menu = new();
menuFactory.AddMenuGroup(nameof(DefaultMenu), menu);

// Attach to the main menu
this.MainMenu.ItemsSource = menuFactory.Items;
```

To conditionally remove menu groups, use the `IMenuFactory.RemoveMenuGroup<T>` method.

```cs
menuFactory.RemoveMenuGroup(nameof(DefaultMenu));
```

## Install

[![NuGet](https://img.shields.io/nuget/v/MenuFactory.svg?label=NuGet&logo=NuGet&labelColor=2A2C33&color=004880&style=for-the-badge)](https://www.nuget.org/packages/MenuFactory) [![NuGet](https://img.shields.io/nuget/dt/MenuFactory.svg?label=NuGet&logo=NuGet&labelColor=2A2C33&color=37c75e&style=for-the-badge)](https://www.nuget.org/packages/MenuFactory)

#### NuGet

```powershell
Install-Package MenuFactory
Install-Package MenuFactory.Abstractions
```

#### Build From Source

```batch
git clone https://github.com/ArchLeaders/MenuFactory
dotnet build MenuFactory/src
```
