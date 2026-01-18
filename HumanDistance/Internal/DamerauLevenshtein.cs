using System.Numerics;
using HumanDistance.Keyboards;

namespace HumanDistance.Internal;

internal static class DamerauLevenshtein
{
    public static int Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
        => CalculateCore(source, target, (a, b) => a == b ? 0 : 1);

    public static double Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target, KeyboardLayoutBase layout)
        => CalculateCore(source, target,
            (a, b) => KeyboardDistanceCalculator.GetSubstitutionCost(a, b, layout));

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
                T cost = getSubstitutionCost(source[i - 1], target[j - 1]);

                T deletion = matrix[i - 1, j] + T.One;
                T insertion = matrix[i, j - 1] + T.One;
                T substitution = matrix[i - 1, j - 1] + cost;

                matrix[i, j] = T.Min(deletion, T.Min(insertion, substitution));

                // Transposition (always cost 1 - timing error, not proximity)
                if (i > 1 && j > 1 &&
                    source[i - 1] == target[j - 2] &&
                    source[i - 2] == target[j - 1])
                {
                    matrix[i, j] = T.Min(matrix[i, j], matrix[i - 2, j - 2] + T.One);
                }
            }
        }

        return matrix[sourceLen, targetLen];
    }
}
