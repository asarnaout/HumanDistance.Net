using System.Numerics;
using HumanDistance.Keyboards;

namespace HumanDistance;

public static class Calculator
{
    private static readonly QwertyLayout DefaultLayout = new();

    public static double Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        return CalculateCore(source, target, (a, b) => GetSubstitutionCost(a, b, DefaultLayout));
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

        return CalculateCore(source, target, (a, b) => GetSubstitutionCost(a, b, layoutInstance));
    }

    private static double GetSubstitutionCost(char a, char b, KeyboardLayoutBase layout)
    {
        if (char.ToLowerInvariant(a) == char.ToLowerInvariant(b))
        {
            return 0.0;
        }

        bool foundA = layout.TryGetPosition(a, out float ax, out float ay);
        bool foundB = layout.TryGetPosition(b, out float bx, out float by);

        if (!foundA || !foundB)
        {
            return 1.0;
        }

        float dx = ax - bx;
        float dy = ay - by;
        float distance = MathF.Sqrt(dx * dx + dy * dy);

        return distance / layout.MaxDistance;
    }

    private static T CalculateCore<T>(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target,
        Func<char, char, T> getSubstitutionCost) where T : INumber<T>
    {
        int sourceLen = source.Length;
        int targetLen = target.Length;

        if (sourceLen == 0) return T.CreateChecked(targetLen);
        if (targetLen == 0) return T.CreateChecked(sourceLen);

        var matrix = new T[sourceLen + 1, targetLen + 1];

        for (int i = 0; i <= sourceLen; i++)
            matrix[i, 0] = T.CreateChecked(i);

        for (int j = 0; j <= targetLen; j++)
            matrix[0, j] = T.CreateChecked(j);

        for (int i = 1; i <= sourceLen; i++)
        {
            for (int j = 1; j <= targetLen; j++)
            {
                T deletion = matrix[i - 1, j] + T.One;
                T insertion = matrix[i, j - 1] + T.One;

                // Check for transposition (adjacent character swap)
                // The last condition ensures it's a real swap, not just repeated characters
                bool isTransposition = i > 1 && j > 1 &&
                    source[i - 1] == target[j - 2] &&
                    source[i - 2] == target[j - 1] &&
                    source[i - 1] != target[j - 1];

                if (isTransposition)
                {
                    // Transposition is a timing/sequencing error, not a key proximity error.
                    // Use fixed transposition cost; don't let substitutions compete.
                    T transposition = matrix[i - 2, j - 2] + T.One;
                    matrix[i, j] = T.Min(deletion, T.Min(insertion, transposition));
                }
                else
                {
                    T cost = getSubstitutionCost(source[i - 1], target[j - 1]);
                    T substitution = matrix[i - 1, j - 1] + cost;
                    matrix[i, j] = T.Min(deletion, T.Min(insertion, substitution));
                }
            }
        }

        return matrix[sourceLen, targetLen];
    }
}
