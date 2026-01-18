using HumanDistance.Keyboards;

namespace HumanDistance;

/// <summary>
/// Calculates edit distance between strings using the Damerau-Levenshtein algorithm
/// with keyboard proximity metrics for substitutions.
/// </summary>
public static class Distance
{
    private static readonly QwertyLayout DefaultLayout = new();

    private enum Operation : byte
    {
        None,
        Insert,
        Delete,
        Substitute,
        Match,
        Transpose
    }

    /// <summary>
    /// Calculates the Damerau-Levenshtein distance between two strings using the QWERTY keyboard layout
    /// for keyboard proximity metrics.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <returns>A <see cref="DistanceResult"/> containing edit distance and operation counts.</returns>
    public static DistanceResult Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        return CalculateInternal(source, target, DefaultLayout);
    }

    /// <summary>
    /// Calculates the Damerau-Levenshtein distance between two strings using the specified keyboard layout
    /// for keyboard proximity metrics.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <param name="layout">The keyboard layout to use for calculating keyboard distances.</param>
    /// <returns>A <see cref="DistanceResult"/> containing edit distance and operation counts.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown keyboard layout is specified.</exception>
    public static DistanceResult Calculate(ReadOnlySpan<char> source, ReadOnlySpan<char> target, KeyboardLayout layout)
    {
        return CalculateInternal(source, target, GetLayoutInstance(layout));
    }

    /// <summary>
    /// Finds the best matching candidate string for the given input.
    /// </summary>
    /// <param name="input">The input string (potentially containing typos).</param>
    /// <param name="candidates">The collection of candidate strings to compare against.</param>
    /// <param name="minScore">The minimum similarity score required for a match (default 0.5).</param>
    /// <param name="keyboardPenaltyStrength">How much keyboard distance affects the score (0.0 = ignore keyboard, 1.0 = maximum penalty). Default is 0.5.</param>
    /// <returns>The best matching candidate, or null if no candidate meets the minimum score.</returns>
    public static string? BestMatch(ReadOnlySpan<char> input, IEnumerable<string> candidates, double minScore = 0.5, double keyboardPenaltyStrength = 0.5)
    {
        return BestMatchInternal(input, candidates, minScore, keyboardPenaltyStrength, DefaultLayout);
    }

    /// <summary>
    /// Finds the best matching candidate string for the given input using the specified keyboard layout.
    /// </summary>
    /// <param name="input">The input string (potentially containing typos).</param>
    /// <param name="candidates">The collection of candidate strings to compare against.</param>
    /// <param name="layout">The keyboard layout to use for calculating keyboard distances.</param>
    /// <param name="minScore">The minimum similarity score required for a match (default 0.5).</param>
    /// <param name="keyboardPenaltyStrength">How much keyboard distance affects the score (0.0 = ignore keyboard, 1.0 = maximum penalty). Default is 0.5.</param>
    /// <returns>The best matching candidate, or null if no candidate meets the minimum score.</returns>
    public static string? BestMatch(ReadOnlySpan<char> input, IEnumerable<string> candidates, KeyboardLayout layout, double minScore = 0.5, double keyboardPenaltyStrength = 0.5)
    {
        return BestMatchInternal(input, candidates, minScore, keyboardPenaltyStrength, GetLayoutInstance(layout));
    }

    private static KeyboardLayoutBase GetLayoutInstance(KeyboardLayout layout)
    {
        return layout switch
        {
            KeyboardLayout.Qwerty => DefaultLayout,
            KeyboardLayout.Azerty => new AzertyLayout(),
            KeyboardLayout.Qwertz => new QwertzLayout(),
            _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, "Unknown keyboard layout")
        };
    }

    private static string? BestMatchInternal(ReadOnlySpan<char> input, IEnumerable<string> candidates, double minScore, double keyboardPenaltyStrength, KeyboardLayoutBase layout)
    {
        string? best = null;
        double bestScore = minScore;

        foreach (var candidate in candidates)
        {
            var result = CalculateInternal(input, candidate, layout);
            var score = result.TypoScore(keyboardPenaltyStrength);
            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private static double GetNormalizedKeyboardDistance(char a, char b, KeyboardLayoutBase layout)
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

    private static DistanceResult CalculateInternal(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target,
        KeyboardLayoutBase layout)
    {
        int sourceLen = source.Length;
        int targetLen = target.Length;

        int maxLength = Math.Max(sourceLen, targetLen);

        // Handle edge cases
        if (sourceLen == 0 && targetLen == 0)
        {
            return new DistanceResult();
        }

        if (sourceLen == 0)
        {
            return new DistanceResult
            {
                EditDistance = targetLen,
                Insertions = targetLen,
                MaxLength = maxLength
            };
        }

        if (targetLen == 0)
        {
            return new DistanceResult
            {
                EditDistance = sourceLen,
                Deletions = sourceLen,
                MaxLength = maxLength
            };
        }

        // Cost matrix (standard Damerau-Levenshtein, all ops = 1)
        var cost = new int[sourceLen + 1, targetLen + 1];
        // Operation matrix for backtracking
        var ops = new Operation[sourceLen + 1, targetLen + 1];

        // Initialize first column (deletions from source)
        for (int i = 0; i <= sourceLen; i++)
        {
            cost[i, 0] = i;
            ops[i, 0] = i > 0 ? Operation.Delete : Operation.None;
        }

        // Initialize first row (insertions to reach target)
        for (int j = 0; j <= targetLen; j++)
        {
            cost[0, j] = j;
            ops[0, j] = j > 0 ? Operation.Insert : Operation.None;
        }

        // Fill the matrices
        for (int i = 1; i <= sourceLen; i++)
        {
            for (int j = 1; j <= targetLen; j++)
            {
                int deletionCost = cost[i - 1, j] + 1;
                int insertionCost = cost[i, j - 1] + 1;

                // Check for transposition (adjacent character swap)
                bool isTransposition = i > 1 && j > 1 &&
                    source[i - 1] == target[j - 2] &&
                    source[i - 2] == target[j - 1] &&
                    source[i - 1] != target[j - 1];

                if (isTransposition)
                {
                    // Transposition is a timing/sequencing error
                    int transpositionCost = cost[i - 2, j - 2] + 1;

                    if (transpositionCost <= deletionCost && transpositionCost <= insertionCost)
                    {
                        cost[i, j] = transpositionCost;
                        ops[i, j] = Operation.Transpose;
                    }
                    else if (deletionCost <= insertionCost)
                    {
                        cost[i, j] = deletionCost;
                        ops[i, j] = Operation.Delete;
                    }
                    else
                    {
                        cost[i, j] = insertionCost;
                        ops[i, j] = Operation.Insert;
                    }
                }
                else
                {
                    // Check for match or substitution
                    bool isMatch = char.ToLowerInvariant(source[i - 1]) == char.ToLowerInvariant(target[j - 1]);
                    int substitutionCost = cost[i - 1, j - 1] + (isMatch ? 0 : 1);

                    if (substitutionCost <= deletionCost && substitutionCost <= insertionCost)
                    {
                        cost[i, j] = substitutionCost;
                        ops[i, j] = isMatch ? Operation.Match : Operation.Substitute;
                    }
                    else if (deletionCost <= insertionCost)
                    {
                        cost[i, j] = deletionCost;
                        ops[i, j] = Operation.Delete;
                    }
                    else
                    {
                        cost[i, j] = insertionCost;
                        ops[i, j] = Operation.Insert;
                    }
                }
            }
        }

        // Backtrack to count operations and compute keyboard distance
        int insertions = 0;
        int deletions = 0;
        int substitutions = 0;
        int transpositions = 0;
        double keyboardDistanceSum = 0.0;

        int si = sourceLen;
        int ti = targetLen;

        while (si > 0 || ti > 0)
        {
            switch (ops[si, ti])
            {
                case Operation.Match:
                    si--;
                    ti--;
                    break;

                case Operation.Substitute:
                    keyboardDistanceSum += GetNormalizedKeyboardDistance(source[si - 1], target[ti - 1], layout);
                    substitutions++;
                    si--;
                    ti--;
                    break;

                case Operation.Insert:
                    insertions++;
                    ti--;
                    break;

                case Operation.Delete:
                    deletions++;
                    si--;
                    break;

                case Operation.Transpose:
                    transpositions++;
                    si -= 2;
                    ti -= 2;
                    break;

                default:
                    // Should not reach here
                    if (ti > 0)
                    {
                        insertions++;
                        ti--;
                    }
                    else if (si > 0)
                    {
                        deletions++;
                        si--;
                    }
                    break;
            }
        }

        return new DistanceResult
        {
            EditDistance = cost[sourceLen, targetLen],
            Insertions = insertions,
            Deletions = deletions,
            Substitutions = substitutions,
            Transpositions = transpositions,
            KeyboardDistanceSum = keyboardDistanceSum,
            MaxLength = maxLength
        };
    }
}
