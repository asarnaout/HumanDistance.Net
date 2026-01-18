namespace HumanDistance;

public readonly struct DistanceResult
{
    /// <summary>
    /// Standard Damerau-Levenshtein edit distance (all operations cost 1).
    /// </summary>
    public int EditDistance { get; init; }

    /// <summary>
    /// Number of insertion operations.
    /// </summary>
    public int Insertions { get; init; }

    /// <summary>
    /// Number of deletion operations.
    /// </summary>
    public int Deletions { get; init; }

    /// <summary>
    /// Number of substitution operations.
    /// </summary>
    public int Substitutions { get; init; }

    /// <summary>
    /// Number of transposition operations.
    /// </summary>
    public int Transpositions { get; init; }

    /// <summary>
    /// Sum of normalized keyboard distances for each substitution.
    /// Each substitution contributes a value from near 0.0 (adjacent keys) to 1.0 (distant or unmapped keys).
    /// </summary>
    public double KeyboardDistanceSum { get; init; }

    internal int MaxLength { get; init; }

    /// <summary>
    /// Average normalized keyboard distance per substitution.
    /// Low values indicate adjacent keys (likely typos), high values indicate distant or unmapped keys.
    /// Returns 0 when there are no substitutions.
    /// </summary>
    public double AverageKeyboardDistance =>
        Substitutions > 0 ? KeyboardDistanceSum / Substitutions : 0.0;

    /// <summary>
    /// Calculates a similarity score from 0.0 to 1.0 where higher values indicate more similarity.
    /// Accounts for both edit distance and keyboard proximity of substituted characters.
    /// Identical strings return 1.0, completely different strings approach 0.0.
    /// </summary>
    /// <param name="keyboardPenaltyStrength">How much keyboard distance affects the score (0.0 = ignore keyboard, 1.0 = maximum penalty). Default is 0.5.</param>
    /// <returns>A similarity score between 0.0 and 1.0.</returns>
    public double TypoScore(double keyboardPenaltyStrength = 0.5)
    {
        if (MaxLength == 0) return 1.0;

        double editSimilarity = 1.0 - ((double)EditDistance / MaxLength);
        if (Substitutions == 0) return editSimilarity;

        double keyboardFactor = 1.0 - (AverageKeyboardDistance * keyboardPenaltyStrength);
        return editSimilarity * keyboardFactor;
    }

    /// <summary>
    /// Determines whether the compared strings are likely typos of each other.
    /// Uses an adaptive threshold based on string length to ensure single-character
    /// typos are detectable regardless of word length.
    /// For custom threshold logic, use <see cref="TypoScore"/> directly.
    /// </summary>
    /// <param name="keyboardPenaltyStrength">How much keyboard distance affects the score (0.0 = ignore keyboard, 1.0 = maximum penalty). Default is 0.5.</param>
    /// <returns>True if TypoScore meets or exceeds the adaptive threshold.</returns>
    public bool IsLikelyTypo(double keyboardPenaltyStrength = 0.5)
    {
        double threshold = GetAdaptiveThreshold(MaxLength);
        return TypoScore(keyboardPenaltyStrength) >= threshold;
    }

    /// <summary>
    /// Calculates an adaptive threshold based on string length.
    /// Shorter strings get a more lenient threshold to ensure single-character typos are detectable.
    /// </summary>
    private static double GetAdaptiveThreshold(int maxLength)
    {
        // For short strings, a single edit is a large percentage of the string.
        // Adjust the threshold to ensure single-edit typos can be detected.
        return maxLength switch
        {
            <= 3 => 0.60,
            4 => 0.70,
            5 => 0.75,
            _ => 0.80
        };
    }
}
