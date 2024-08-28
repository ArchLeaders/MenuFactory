using MenuFactory.Abstractions.Attributes;
using System.Diagnostics;

namespace MenuFactory.Sample.Models;

internal class ExtrasMenu
{
    [Menu("Extra Example", "Extras", Icon = "fa-square-poll-vertical", InputGesture = "Ctrl + Alt + F1")]
    public static void ExtraExample()
    {
        Debugger.Break();
    }

    [Menu("Example 4b2 (Extra)", "Examples/Nested 2/More", Icon = "fa-star-and-crescent", InputGesture = "Ctrl + Alt + F2")]
    public static void Example4b2()
    {
        Debugger.Break();
    }

    [Menu("Example 4c (Extra)", "Examples/Nested 2/More (Extras)$:Icon=fa-flag", Icon = "fa-moon", InputGesture = "Ctrl + Alt + F3")]
    public static void Example4c()
    {
        Debugger.Break();
    }
}
