using HumanDistance.Internal;

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

        var layout = KeyboardDistanceCalculator.CreateLayout(options.Layout);

        return DamerauLevenshtein.Calculate(source, target, layout);
    }
}
