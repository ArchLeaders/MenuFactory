using MenuFactory.Abstractions.Attributes;
using System.Diagnostics;

namespace MenuFactory.Sample.Models;

internal class DefaultMenu
{
    [Menu("Open File", "File", Icon = "fa-file", InputGesture = "Ctrl + O")]
    public static async Task OpenFile()
    {
        // Async methods work!
        await Task.CompletedTask;

        Debugger.Break();
    }

    [Menu("Exit", "File", Icon = "fa-arrow-right-from-bracket", InputGesture = "Ctrl + W", IsSeparator = true)]
    public static void Exit()
    {
        Environment.Exit(0);
    }

    [Menu("Example 1", "Examples", Icon = "fa-star")]
    public static void Example1()
    {
        Debugger.Break();
    }

    [Menu("Example 2", "Examples", Icon = "fa-layer-group", InputGesture = "Ctrl + Shift + X", IsSeparator = true)]
    public static void Example2()
    {
        Debugger.Break();
    }

    [Menu("Example 3a", "Examples/Nested$:Icon=fa-tags", Icon = "fa-leaf", InputGesture = "Ctrl + F1")]
    public static void Example3a()
    {
        Debugger.Break();
    }

    [Menu("Example 3b", "Examples/Nested", Icon = "fa-seedling", InputGesture = "Ctrl + F2")]
    public static void Example3b()
    {
        Debugger.Break();
    }

    [Menu("Example 4a", "Examples/Nested 2$:Icon=fa-expand", Icon = "fa-marker", InputGesture = "Ctrl + F3")]
    public static void Example4a()
    {
        Debugger.Break();
    }

    [Menu("Example 4b", "Examples/Nested 2", Icon = "fa-pencil", InputGesture = "Ctrl + F4")]
    public static void Example4b()
    {
        Debugger.Break();
    }

    [Menu("Example 4b1", "Examples/Nested 2/More$:Icon=fa-repeat,IsSeparator=true", Icon = "fa-crown", InputGesture = "Ctrl + F5")]
    public static void Example4b1()
    {
        Debugger.Break();
    }
}
