# Menu Factory

[![License](https://img.shields.io/badge/License-MIT-blue.svg?logo=github&logoColor=5751ff&labelColor=2A2C33&color=5751ff&style=for-the-badge)](https://github.com/ArchLeaders/MenuFactory/blob/master/License.md)

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

See the [samples](./samples) for more usage details.

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
