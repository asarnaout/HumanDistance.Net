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
    /// A similarity score from 0.0 to 1.0 where higher values indicate more similarity.
    /// Accounts for both edit distance and keyboard proximity of substituted characters.
    /// Identical strings return 1.0, completely different strings approach 0.0.
    /// </summary>
    public double TypoScore
    {
        get
        {
            if (MaxLength == 0)
            {
                return 1.0;
            }

            double normalizedSimilarity = 1.0 - ((double)EditDistance / MaxLength);
            double keyboardPenalty = KeyboardDistanceSum / MaxLength;

            return Math.Max(0.0, normalizedSimilarity - keyboardPenalty);
        }
    }

    /// <summary>
    /// Determines whether the compared strings are likely typos of each other.
    /// </summary>
    /// <param name="threshold">The minimum similarity score to consider a typo (default 0.8).</param>
    /// <returns>True if TypoScore meets or exceeds the threshold.</returns>
    public bool IsLikelyTypo(double threshold = 0.8) => TypoScore >= threshold;
}
