using HumanDistance.Keyboards;

namespace HumanDistance;

public static class Calculator
{
    public static int Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        return DamerauLevenshtein.Calculate(source, target);
    }

    public static double Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target, CalculatorOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!options.UseKeyboardDistance)
        {
            return DamerauLevenshtein.Calculate(source, target);
        }

        KeyboardLayoutBase layout = options.Layout switch
        {
            KeyboardLayout.Qwerty => new QwertyLayout(),
            KeyboardLayout.Azerty => new AzertyLayout(),
            KeyboardLayout.Qwertz => new QwertzLayout(),
            _ => throw new ArgumentOutOfRangeException(nameof(options), options.Layout, "Unknown keyboard layout")
        };

        return DamerauLevenshtein.Calculate(source, target, layout);
    }
}
