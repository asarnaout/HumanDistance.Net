using HumanDistance.Keyboards;

namespace HumanDistance.Internal;

internal static class DamerauLevenshtein
{
    public static int Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        int sourceLen = source.Length;
        int targetLen = target.Length;

        if (sourceLen == 0) return targetLen;
        if (targetLen == 0) return sourceLen;

        // Use full matrix for transposition support
        var matrix = new int[sourceLen + 1, targetLen + 1];

        for (int i = 0; i <= sourceLen; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= targetLen; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= sourceLen; i++)
        {
            for (int j = 1; j <= targetLen; j++)
            {
                int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                int deletion = matrix[i - 1, j] + 1;
                int insertion = matrix[i, j - 1] + 1;
                int substitution = matrix[i - 1, j - 1] + cost;

                matrix[i, j] = Math.Min(deletion, Math.Min(insertion, substitution));

                // Transposition
                if (i > 1 && j > 1 &&
                    source[i - 1] == target[j - 2] &&
                    source[i - 2] == target[j - 1])
                {
                    matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + 1);
                }
            }
        }

        return matrix[sourceLen, targetLen];
    }

    public static double Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target, KeyboardLayoutBase layout)
    {
        int sourceLen = source.Length;
        int targetLen = target.Length;

        if (sourceLen == 0) return targetLen;
        if (targetLen == 0) return sourceLen;

        // Use full matrix for transposition support
        var matrix = new double[sourceLen + 1, targetLen + 1];

        for (int i = 0; i <= sourceLen; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= targetLen; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= sourceLen; i++)
        {
            for (int j = 1; j <= targetLen; j++)
            {
                double substitutionCost = KeyboardDistanceCalculator.GetSubstitutionCost(
                    source[i - 1], target[j - 1], layout);

                double deletion = matrix[i - 1, j] + 1.0;
                double insertion = matrix[i, j - 1] + 1.0;
                double substitution = matrix[i - 1, j - 1] + substitutionCost;

                matrix[i, j] = Math.Min(deletion, Math.Min(insertion, substitution));

                // Transposition (always cost 1 - timing error, not proximity)
                if (i > 1 && j > 1 &&
                    source[i - 1] == target[j - 2] &&
                    source[i - 2] == target[j - 1])
                {
                    matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + 1.0);
                }
            }
        }

        return matrix[sourceLen, targetLen];
    }
}
