using HumanDistance.Keyboards;

namespace HumanDistance;

public static class Calculator
{
    private static readonly QwertyLayout DefaultLayout = new();

    public static double Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        return DamerauLevenshtein.Calculate(source, target, DefaultLayout);
    }

    public static double Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target, KeyboardLayout layout)
    {
        KeyboardLayoutBase layoutInstance = layout switch
        {
            KeyboardLayout.Qwerty => DefaultLayout,
            KeyboardLayout.Azerty => new AzertyLayout(),
            KeyboardLayout.Qwertz => new QwertzLayout(),
            _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, "Unknown keyboard layout")
        };

        return DamerauLevenshtein.Calculate(source, target, layoutInstance);
    }
}
