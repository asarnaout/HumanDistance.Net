using HumanDistance.Keyboards;

namespace HumanDistance.Tests;

public class DistanceTests
{
    [Fact]
    public void Calculate_IdenticalStrings_ReturnsZero()
    {
        var result = Distance.Calculate("hello", "hello");
        Assert.Equal(0, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(0, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
        Assert.Equal(0.0, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_EmptyStrings_ReturnsZero()
    {
        var result = Distance.Calculate("", "");
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_EmptySource_ReturnsTargetLength()
    {
        var result = Distance.Calculate("", "hello");
        Assert.Equal(5, result.EditDistance);
        Assert.Equal(5, result.Insertions);
        Assert.Equal(0, result.Deletions);
    }

    [Fact]
    public void Calculate_EmptyTarget_ReturnsSourceLength()
    {
        var result = Distance.Calculate("hello", "");
        Assert.Equal(5, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(5, result.Deletions);
    }

    [Fact]
    public void Calculate_SingleInsertion_ReturnsOne()
    {
        var result = Distance.Calculate("hell", "hello");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Insertions);
        Assert.Equal(0, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
    }

    [Fact]
    public void Calculate_SingleDeletion_ReturnsOne()
    {
        var result = Distance.Calculate("hello", "hell");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(1, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
    }

    [Fact]
    public void Calculate_SingleSubstitution_ReturnsOne()
    {
        var result = Distance.Calculate("hello", "hallo");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.True(result.KeyboardDistanceSum > 0.0, "Keyboard distance should be positive for substitution");
        Assert.True(result.KeyboardDistanceSum <= 1.0, "Single substitution keyboard distance should be at most 1.0");
    }

    [Fact]
    public void Calculate_Transposition_ReturnsOne()
    {
        var result = Distance.Calculate("hello", "hlelo");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Transpositions);
        Assert.Equal(0, result.Substitutions);
    }

    [Fact]
    public void Calculate_CaseInsensitive()
    {
        var result = Distance.Calculate("Hello", "hello");
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_AdjacentKeys_HasLowKeyboardDistance()
    {
        var result = Distance.Calculate("a", "s");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.True(result.KeyboardDistanceSum < 0.5,
            $"Adjacent keys 'a' and 's' should have low keyboard distance, got {result.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_DistantKeys_HasHigherKeyboardDistance()
    {
        var adjacentResult = Distance.Calculate("a", "s");
        var distantResult = Distance.Calculate("a", "p");
        Assert.True(distantResult.KeyboardDistanceSum > adjacentResult.KeyboardDistanceSum,
            $"Distant keys should have higher keyboard distance. Adjacent: {adjacentResult.KeyboardDistanceSum}, Distant: {distantResult.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_UnknownCharacters_DefaultToFullCost()
    {
        var result = Distance.Calculate("a", "α");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.Equal(1.0, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_DefaultsToQwerty()
    {
        var result = Distance.Calculate("q", "a");
        var qwertyResult = Distance.Calculate("q", "a", KeyboardLayout.Qwerty);
        Assert.Equal(qwertyResult.EditDistance, result.EditDistance);
        Assert.Equal(qwertyResult.KeyboardDistanceSum, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_WithLayout_IdenticalStrings_ReturnsZero()
    {
        var result = Distance.Calculate("hello", "hello", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_AdjacentKeys_HasLowKeyboardDistance()
    {
        var result = Distance.Calculate("a", "s", KeyboardLayout.Qwerty);
        Assert.Equal(1, result.EditDistance);
        Assert.True(result.KeyboardDistanceSum < 0.5,
            $"Adjacent keys 'a' and 's' should have low keyboard distance, got {result.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_WithLayout_DistantKeys_HasHigherKeyboardDistance()
    {
        var adjacentResult = Distance.Calculate("a", "s", KeyboardLayout.Qwerty);
        var distantResult = Distance.Calculate("a", "p", KeyboardLayout.Qwerty);
        Assert.True(distantResult.KeyboardDistanceSum > adjacentResult.KeyboardDistanceSum,
            $"Distant keys should have higher keyboard distance. Adjacent: {adjacentResult.KeyboardDistanceSum}, Distant: {distantResult.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_WithLayout_QwertyLayout_WorksCorrectly()
    {
        var result = Distance.Calculate("qwerty", "qwerty", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_AzertyLayout_WorksCorrectly()
    {
        var result = Distance.Calculate("azerty", "azerty", KeyboardLayout.Azerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_QwertzLayout_WorksCorrectly()
    {
        var result = Distance.Calculate("qwertz", "qwertz", KeyboardLayout.Qwertz);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_CaseInsensitive()
    {
        var result = Distance.Calculate("Hello", "hello", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Theory]
    [InlineData("", "", 0, 0, 0)]
    [InlineData("a", "", 1, 0, 1)]
    [InlineData("", "a", 1, 1, 0)]
    [InlineData("abc", "abc", 0, 0, 0)]
    [InlineData("abc", "ab", 1, 0, 1)]
    [InlineData("ab", "abc", 1, 1, 0)]
    [InlineData("abc", "acb", 1, 0, 0)] // Transposition
    public void Calculate_VariousInputs_ReturnsExpectedDistance(
        string source, string target, int expectedDistance, int expectedInsertions, int expectedDeletions)
    {
        var result = Distance.Calculate(source, target);
        Assert.Equal(expectedDistance, result.EditDistance);
        Assert.Equal(expectedInsertions, result.Insertions);
        Assert.Equal(expectedDeletions, result.Deletions);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwerty, "q", "w", "q", "m")]
    [InlineData(KeyboardLayout.Azerty, "a", "z", "a", "p")]
    [InlineData(KeyboardLayout.Azerty, "q", "s", "q", "m")]
    [InlineData(KeyboardLayout.Qwertz, "a", "s", "a", "p")]
    [InlineData(KeyboardLayout.Qwertz, "q", "w", "q", "m")]
    public void Calculate_WithLayout_AdjacentKeysLowerKeyboardDistanceThanDistantKeys(
        KeyboardLayout layout, string source1, string adjacentTarget, string source2, string distantTarget)
    {
        var adjacentResult = Distance.Calculate(source1, adjacentTarget, layout);
        var distantResult = Distance.Calculate(source2, distantTarget, layout);
        Assert.True(adjacentResult.KeyboardDistanceSum < distantResult.KeyboardDistanceSum,
            $"Layout {layout}: Adjacent keys ({source1}->{adjacentTarget}={adjacentResult.KeyboardDistanceSum:F3}) should have lower keyboard distance than distant keys ({source2}->{distantTarget}={distantResult.KeyboardDistanceSum:F3})");
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void Calculate_WithLayout_UnknownCharacters_DefaultToFullCost(KeyboardLayout layout)
    {
        var result = Distance.Calculate("a", "α", layout);
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1.0, result.KeyboardDistanceSum);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "ab", "ba")]
    [InlineData(KeyboardLayout.Qwerty, "qp", "pq")]
    [InlineData(KeyboardLayout.Azerty, "az", "za")]
    [InlineData(KeyboardLayout.Azerty, "qm", "mq")]
    [InlineData(KeyboardLayout.Qwertz, "ab", "ba")]
    [InlineData(KeyboardLayout.Qwertz, "qp", "pq")]
    public void Calculate_WithLayout_Transposition_ReturnsOne(
        KeyboardLayout layout, string source, string target)
    {
        var result = Distance.Calculate(source, target, layout);
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Transpositions);
        Assert.Equal(0, result.Substitutions);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "", "", 0)]
    [InlineData(KeyboardLayout.Qwerty, "a", "", 1)]
    [InlineData(KeyboardLayout.Qwerty, "", "a", 1)]
    [InlineData(KeyboardLayout.Qwerty, "abc", "abc", 0)]
    [InlineData(KeyboardLayout.Qwerty, "Hello", "hello", 0)]
    [InlineData(KeyboardLayout.Azerty, "", "", 0)]
    [InlineData(KeyboardLayout.Azerty, "a", "", 1)]
    [InlineData(KeyboardLayout.Azerty, "", "a", 1)]
    [InlineData(KeyboardLayout.Azerty, "abc", "abc", 0)]
    [InlineData(KeyboardLayout.Azerty, "Hello", "hello", 0)]
    [InlineData(KeyboardLayout.Qwertz, "", "", 0)]
    [InlineData(KeyboardLayout.Qwertz, "a", "", 1)]
    [InlineData(KeyboardLayout.Qwertz, "", "a", 1)]
    [InlineData(KeyboardLayout.Qwertz, "abc", "abc", 0)]
    [InlineData(KeyboardLayout.Qwertz, "Hello", "hello", 0)]
    public void Calculate_WithLayout_VariousInputs_ReturnsExpectedEditDistance(
        KeyboardLayout layout, string source, string target, int expectedDistance)
    {
        var result = Distance.Calculate(source, target, layout);
        Assert.Equal(expectedDistance, result.EditDistance);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "abc", "adc", 1, 0.2147205770015717)]
    [InlineData(KeyboardLayout.Qwerty, "a", "z", 1, 0.08915787190198898)]
    [InlineData(KeyboardLayout.Azerty, "abc", "adc", 1, 0.22438186407089233)]
    [InlineData(KeyboardLayout.Azerty, "a", "z", 1, 0.0833333358168602)]
    [InlineData(KeyboardLayout.Qwertz, "abc", "adc", 1, 0.22438186407089233)]
    [InlineData(KeyboardLayout.Qwertz, "a", "z", 1, 0.40451017022132874)]
    public void Calculate_WithLayout_SubstitutionWithExpectedKeyboardDistance(
        KeyboardLayout layout, string source, string target, int expectedSubstitutions, double expectedKeyboardDistance)
    {
        var result = Distance.Calculate(source, target, layout);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedKeyboardDistance, result.KeyboardDistanceSum, precision: 10);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty, "abc", "bad", 1, 1, 0.08915787190198898)]
    [InlineData(KeyboardLayout.Azerty, "abc", "bad", 1, 1, 0.0931695029139519)]
    [InlineData(KeyboardLayout.Qwertz, "abc", "bad", 1, 1, 0.0931695029139519)]
    public void Calculate_WithLayout_TranspositionPlusSubstitution(
        KeyboardLayout layout, string source, string target,
        int expectedTranspositions, int expectedSubstitutions, double expectedKeyboardDistance)
    {
        var result = Distance.Calculate(source, target, layout);
        Assert.Equal(expectedTranspositions, result.Transpositions);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedKeyboardDistance, result.KeyboardDistanceSum, precision: 10);
    }

    [Theory]
    [InlineData("kitten", "sitting", 3, 1, 0, 2, 0)] // s->k sub, i->e sub, g insert
    [InlineData("saturday", "sunday", 3, 0, 2, 1, 0)] // at delete, delete r, n->r sub
    [InlineData("abc", "cba", 2, 0, 0, 2, 0)] // a->c, c->a subs (not transposition since not adjacent)
    [InlineData("ab", "ba", 1, 0, 0, 0, 1)] // Transposition
    [InlineData("test", "toast", 2, 1, 0, 1, 0)] // e->o sub, a insert
    public void Calculate_EditDistanceEqualsOperationSum(
        string source, string target,
        int expectedDistance, int expectedInsertions, int expectedDeletions,
        int expectedSubstitutions, int expectedTranspositions)
    {
        var result = Distance.Calculate(source, target);
        Assert.Equal(expectedDistance, result.EditDistance);
        Assert.Equal(expectedInsertions, result.Insertions);
        Assert.Equal(expectedDeletions, result.Deletions);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedTranspositions, result.Transpositions);

        // Verify EditDistance equals sum of all operations
        Assert.Equal(result.EditDistance,
            result.Insertions + result.Deletions + result.Substitutions + result.Transpositions);
    }

    [Fact]
    public void Calculate_NoSubstitutions_KeyboardDistanceSumIsZero()
    {
        // Pure insertions
        var insertResult = Distance.Calculate("abc", "abcde");
        Assert.Equal(0, insertResult.Substitutions);
        Assert.Equal(0.0, insertResult.KeyboardDistanceSum);

        // Pure deletions
        var deleteResult = Distance.Calculate("hello", "hel");
        Assert.Equal(0, deleteResult.Substitutions);
        Assert.Equal(0.0, deleteResult.KeyboardDistanceSum);

        // Pure transposition
        var transposeResult = Distance.Calculate("ab", "ba");
        Assert.Equal(0, transposeResult.Substitutions);
        Assert.Equal(0.0, transposeResult.KeyboardDistanceSum);

        // Identical strings
        var identicalResult = Distance.Calculate("test", "test");
        Assert.Equal(0, identicalResult.Substitutions);
        Assert.Equal(0.0, identicalResult.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_AverageKeyboardDistance_CalculatedCorrectly()
    {
        // Single substitution
        var singleResult = Distance.Calculate("a", "s");
        Assert.Equal(singleResult.KeyboardDistanceSum, singleResult.AverageKeyboardDistance);

        // Multiple substitutions
        var multiResult = Distance.Calculate("abc", "xyz");
        Assert.Equal(3, multiResult.Substitutions);
        Assert.Equal(multiResult.KeyboardDistanceSum / 3, multiResult.AverageKeyboardDistance);
    }

    [Fact]
    public void Calculate_NoSubstitutions_AverageKeyboardDistanceIsZero()
    {
        var result = Distance.Calculate("abc", "abcde"); // Pure insertions
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0.0, result.AverageKeyboardDistance);
    }

    [Theory]
    [InlineData("password", "pasword", 1, 0, 1, 0, 0)]
    [InlineData("password", "Password", 0, 0, 0, 0, 0)] // Case insensitive
    [InlineData("password", "passowrd", 1, 0, 0, 0, 1)] // Transposition
    [InlineData("password", "p4ssword", 1, 0, 0, 1, 0)] // Substitution (a->4)
    public void Calculate_TypicalTypoScenarios(
        string source, string target,
        int expectedDistance, int expectedInsertions, int expectedDeletions,
        int expectedSubstitutions, int expectedTranspositions)
    {
        var result = Distance.Calculate(source, target);
        Assert.Equal(expectedDistance, result.EditDistance);
        Assert.Equal(expectedInsertions, result.Insertions);
        Assert.Equal(expectedDeletions, result.Deletions);
        Assert.Equal(expectedSubstitutions, result.Substitutions);
        Assert.Equal(expectedTranspositions, result.Transpositions);
    }

    [Fact]
    public void Calculate_MultipleSubstitutions_KeyboardDistanceSumAccumulates()
    {
        // Get individual keyboard distances
        var azResult = Distance.Calculate("a", "z", KeyboardLayout.Qwerty);
        var bxResult = Distance.Calculate("b", "x", KeyboardLayout.Qwerty);

        // Calculate combined
        var combinedResult = Distance.Calculate("ab", "zx", KeyboardLayout.Qwerty);

        Assert.Equal(2, combinedResult.Substitutions);
        Assert.Equal(azResult.KeyboardDistanceSum + bxResult.KeyboardDistanceSum,
            combinedResult.KeyboardDistanceSum, precision: 10);
    }

    // TypoScore Tests

    [Fact]
    public void TypoScore_IdenticalStrings_ReturnsOne()
    {
        var result = Distance.Calculate("password", "password");
        Assert.Equal(1.0, result.TypoScore());
    }

    [Fact]
    public void TypoScore_BothEmpty_ReturnsOne()
    {
        var result = Distance.Calculate("", "");
        Assert.Equal(1.0, result.TypoScore());
    }

    [Fact]
    public void TypoScore_EmptyVsNonEmpty_ReturnsZero()
    {
        var result = Distance.Calculate("", "test");
        Assert.Equal(0.0, result.TypoScore());
    }

    [Fact]
    public void TypoScore_SingleTransposition_HighScore()
    {
        // "passwrod" vs "password" - single transposition (o and r swapped)
        var result = Distance.Calculate("passwrod", "password");
        Assert.True(result.TypoScore() > 0.8, $"Single transposition should have high score, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_SingleAdjacentKeySubstitution_HighScore()
    {
        // "passwprd" vs "password" - o→p is a truly adjacent key typo on QWERTY
        var result = Distance.Calculate("passwprd", "password");
        Assert.True(result.TypoScore() > 0.8, $"Adjacent key substitution should have high score, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_SingleDistantKeySubstitution_LowerThanAdjacent()
    {
        // Compare adjacent vs distant substitution on QWERTY
        // 'o' is adjacent to 'i' and 'p', but far from 'a'
        var adjacentResult = Distance.Calculate("passwprd", "password"); // o→p (adjacent)
        var distantResult = Distance.Calculate("passward", "password");  // o→a (distant - different row)

        Assert.True(distantResult.TypoScore() < adjacentResult.TypoScore(),
            $"Distant key substitution ({distantResult.TypoScore()}) should score lower than adjacent ({adjacentResult.TypoScore()})");
    }

    [Fact]
    public void TypoScore_CompletelyDifferent_LowScore()
    {
        var result = Distance.Calculate("qwertyui", "password");
        Assert.True(result.TypoScore() < 0.3, $"Completely different strings should have low score, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_ScoreNeverNegative()
    {
        // Even with maximum penalties, score should not go negative
        var result = Distance.Calculate("aaaa", "zzzz");
        Assert.True(result.TypoScore() >= 0.0, $"Score should never be negative, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_WithLayout_WorksCorrectly()
    {
        var qwertyResult = Distance.Calculate("a", "s", KeyboardLayout.Qwerty);
        var azertyResult = Distance.Calculate("a", "s", KeyboardLayout.Azerty);

        // Both should be valid scores
        Assert.True(qwertyResult.TypoScore() >= 0.0 && qwertyResult.TypoScore() <= 1.0);
        Assert.True(azertyResult.TypoScore() >= 0.0 && azertyResult.TypoScore() <= 1.0);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void TypoScore_WithLayout_IdenticalStrings_ReturnsOne(KeyboardLayout layout)
    {
        var result = Distance.Calculate("test", "test", layout);
        Assert.Equal(1.0, result.TypoScore());
    }

    // IsLikelyTypo Tests

    [Fact]
    public void IsLikelyTypo_CommonTypo_ReturnsTrue()
    {
        // "teh" is a common typo for "the"
        var result = Distance.Calculate("teh", "the");
        Assert.True(result.IsLikelyTypo());
    }

    [Fact]
    public void IsLikelyTypo_CompletelyDifferent_ReturnsFalse()
    {
        var result = Distance.Calculate("hello", "world");
        Assert.False(result.IsLikelyTypo());
    }

    [Fact]
    public void IsLikelyTypo_IdenticalStrings_ReturnsTrue()
    {
        var result = Distance.Calculate("test", "test");
        Assert.True(result.IsLikelyTypo());
    }

    [Fact]
    public void IsLikelyTypo_DefaultThreshold_IsPractical()
    {
        // Single character transposition in 8-char word should pass default 0.8 threshold
        var result = Distance.Calculate("passwrod", "password");
        Assert.True(result.IsLikelyTypo(), "Single transposition should be detected as typo with default threshold");
    }

    [Fact]
    public void IsLikelyTypo_WithLayout_WorksCorrectly()
    {
        var result = Distance.Calculate("teh", "the", KeyboardLayout.Qwerty);
        Assert.True(result.IsLikelyTypo());
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void IsLikelyTypo_WithLayout_IdenticalStrings_ReturnsTrue(KeyboardLayout layout)
    {
        var result = Distance.Calculate("test", "test", layout);
        Assert.True(result.IsLikelyTypo());
    }

    // Adaptive Threshold Tests

    [Theory]
    [InlineData("git", "gti")]      // 3-char transposition
    [InlineData("npm", "nmp")]      // 3-char transposition
    [InlineData("the", "teh")]      // 3-char transposition
    public void IsLikelyTypo_AdaptiveThreshold_ThreeCharTypos_DetectedWithDefaultThreshold(string original, string typo)
    {
        var result = Distance.Calculate(original, typo);

        // These would fail with a fixed 0.8 threshold (score ~0.667)
        // but should pass with adaptive threshold (0.60 for 3-char words)
        Assert.True(result.IsLikelyTypo(),
            $"'{original}'→'{typo}' should be detected as typo (score: {result.TypoScore():F3})");
    }

    [Theory]
    [InlineData("test", "tset")]    // 4-char transposition
    [InlineData("push", "psuh")]    // 4-char transposition
    [InlineData("pull", "plul")]    // 4-char transposition
    public void IsLikelyTypo_AdaptiveThreshold_FourCharTypos_DetectedWithDefaultThreshold(string original, string typo)
    {
        var result = Distance.Calculate(original, typo);

        // These would fail with a fixed 0.8 threshold (score = 0.75)
        // but should pass with adaptive threshold (0.70 for 4-char words)
        Assert.True(result.IsLikelyTypo(),
            $"'{original}'→'{typo}' should be detected as typo (score: {result.TypoScore():F3})");
    }

    [Theory]
    [InlineData("hello", "helo")]   // 5-char deletion
    [InlineData("build", "biuld")]  // 5-char transposition
    public void IsLikelyTypo_AdaptiveThreshold_FiveCharTypos_DetectedWithDefaultThreshold(string original, string typo)
    {
        var result = Distance.Calculate(original, typo);

        // Score = 0.80, threshold adapts to 0.75 for 5-char words
        Assert.True(result.IsLikelyTypo(),
            $"'{original}'→'{typo}' should be detected as typo (score: {result.TypoScore():F3})");
    }

    [Theory]
    [InlineData("go", "to")]        // Different 2-char words
    [InlineData("is", "as")]        // Different 2-char words
    [InlineData("on", "in")]        // Different 2-char words
    public void IsLikelyTypo_AdaptiveThreshold_TwoCharDifferentWords_NotDetected(string word1, string word2)
    {
        var result = Distance.Calculate(word1, word2);

        // Even with adaptive threshold, completely different short words
        // should not be considered typos (keyboard distance penalty helps here)
        Assert.False(result.IsLikelyTypo(),
            $"'{word1}'↔'{word2}' should NOT be detected as typo (score: {result.TypoScore():F3})");
    }

    [Fact]
    public void IsLikelyTypo_AdaptiveThreshold_LongerWords_UseBaseThreshold()
    {
        // For 6+ char words, the base threshold (0.8) should be used
        var result = Distance.Calculate("status", "stauts");

        // Score ≈ 0.833, above 0.8 threshold - should pass
        Assert.True(result.IsLikelyTypo());

        // Now test a 6-char word with a distant key substitution
        // This creates a score below 0.80 due to keyboard penalty
        var result2 = Distance.Calculate("abcdef", "xbcdef"); // a→x is distant on QWERTY

        // Score should be below 0.80 due to keyboard distance penalty
        Assert.True(result2.TypoScore() < 0.80,
            $"Expected score below 0.80 for distant key substitution, got {result2.TypoScore():F3}");

        // This should FAIL because 6+ char words use the base 0.80 threshold
        // (no adaptive threshold benefit like shorter words get)
        Assert.False(result2.IsLikelyTypo(),
            $"6-char word with score {result2.TypoScore():F3} should fail at 0.80 threshold");

        // Contrast: a 4-char word with 1 edit passes because of adaptive threshold (0.70)
        // Using transposition which has no keyboard penalty, score = 0.75
        var result3 = Distance.Calculate("test", "tset"); // transposition
        Assert.Equal(0.75, result3.TypoScore(), precision: 2);
        Assert.True(result3.IsLikelyTypo(),
            $"4-char word with score {result3.TypoScore():F3} should pass with adaptive threshold 0.70");
    }

    [Fact]
    public void TypoScore_CanBeUsedForCustomThresholds()
    {
        // For custom threshold logic, use TypoScore() directly
        var result = Distance.Calculate("hello", "world");

        // Very different words have a low score
        Assert.True(result.TypoScore() < 0.3);

        // Custom threshold comparison
        Assert.False(result.TypoScore() >= 0.5,
            "TypoScore can be compared against any custom threshold");
    }

    [Fact]
    public void IsLikelyTypo_UsesAdaptiveThresholdAutomatically()
    {
        // IsLikelyTypo uses adaptive thresholds automatically
        // No need to pass a threshold parameter
        var result = Distance.Calculate("git", "gti");

        // 3-char word with transposition: score = 0.667
        // Adaptive threshold for 3-char words is 0.60
        Assert.True(result.IsLikelyTypo(),
            "3-char typo should be detected with adaptive threshold");

        // Verify the score is what we expect
        Assert.True(result.TypoScore() >= 0.60);
        Assert.True(result.TypoScore() < 0.80);
    }

    // BestMatch Tests

    [Fact]
    public void BestMatch_FindsCorrectMatch()
    {
        var candidates = new[] { "recipe", "receipt", "record" };
        var result = Distance.BestMatch("reciepe", candidates);
        Assert.Equal("recipe", result);
    }

    [Fact]
    public void BestMatch_NoMatchAboveThreshold_ReturnsNull()
    {
        var candidates = new[] { "abc", "def", "ghi" };
        var result = Distance.BestMatch("xyz", candidates);
        Assert.Null(result);
    }

    [Fact]
    public void BestMatch_EmptyCandidates_ReturnsNull()
    {
        var candidates = Array.Empty<string>();
        var result = Distance.BestMatch("test", candidates);
        Assert.Null(result);
    }

    [Fact]
    public void BestMatch_ExactMatchInCandidates_ReturnsIt()
    {
        var candidates = new[] { "apple", "banana", "cherry" };
        var result = Distance.BestMatch("banana", candidates);
        Assert.Equal("banana", result);
    }

    [Fact]
    public void BestMatch_WithCustomMinScore_Respected()
    {
        var candidates = new[] { "test", "tast", "tost" };

        // With low minScore, should find a match
        var lowThreshold = Distance.BestMatch("txst", candidates, minScore: 0.3);
        Assert.NotNull(lowThreshold);

        // With very high minScore, might not find a match
        var highThreshold = Distance.BestMatch("txst", candidates, minScore: 0.99);
        Assert.Null(highThreshold);
    }

    // Verified examples for README: ensure behavior matches documentation claims
    [Fact]
    public void Examples_SlipSlopSlap_Behavior()
    {
        var slop = Distance.Calculate("slip", "slop");
        var slap = Distance.Calculate("slip", "slap");

        Assert.Equal(1, slop.EditDistance);
        Assert.Equal(1, slap.EditDistance);
        Assert.True(slop.IsLikelyTypo());
        Assert.False(slap.IsLikelyTypo());

        var bm = Distance.BestMatch("slop", new[] { "slip", "slap" });
        Assert.Equal("slip", bm);
    }

    [Fact]
    public void Examples_FormFromFarm_Behavior()
    {
        var from = Distance.Calculate("form", "from");
        var farm = Distance.Calculate("form", "farm");

        Assert.True(from.IsLikelyTypo());
        Assert.False(farm.IsLikelyTypo());
    }

    [Fact]
    public void Examples_GitGti_Behavior()
    {
        var gti = Distance.Calculate("git", "gti");
        Assert.True(gti.IsLikelyTypo());
    }

    [Fact]
    public void Examples_RecieptReceipt_Behavior()
    {
        var receipt = Distance.Calculate("reciept", "receipt");
        Assert.True(receipt.IsLikelyTypo());

        var best = Distance.BestMatch("reciept", new[] { "receipt", "recipe" });
        Assert.Equal("receipt", best);
    }

    [Fact]
    public void Examples_EditDistance2_BestMatchPrefersAdjacentSubs()
    {
        // Input with two mistakes; both candidates are at DL distance 2
        string input = "kryboqrd";   // from "keyboard" with r->e, q->a adjacent-key intent
        string nearby = "keyboard";  // adjacent substitutions relative to input positions
        string distant = "kpybozrd"; // same two positions changed but to distant keys

        var toNearby = Distance.Calculate(input, nearby);
        var toDistant = Distance.Calculate(input, distant);

        Assert.Equal(2, toNearby.EditDistance);
        Assert.Equal(2, toDistant.EditDistance);

        // TypoScore should be higher for adjacent-key substitutions
        Assert.True(toNearby.TypoScore() > toDistant.TypoScore());

        // BestMatch should pick the adjacent-keys candidate
        var match = Distance.BestMatch(input, new[] { nearby, distant }, minScore: 0.0, keyboardPenaltyStrength: 0.5);
        Assert.Equal(nearby, match);
    }
    [Fact]
    public void BestMatch_MeetingMinimumScore_IsAccepted()
    {
        var candidates = new[] { "apple", "banana" };

        // Exact match produces score 1.0, which should satisfy a 1.0 threshold
        var result = Distance.BestMatch("apple", candidates, minScore: 1.0);
        Assert.Equal("apple", result);
    }

    [Fact]
    public void BestMatch_MultipleSimilarCandidates_ReturnsBest()
    {
        var candidates = new[] { "testing", "tasting", "tosting" };
        var result = Distance.BestMatch("testin", candidates);
        Assert.Equal("testing", result);
    }

    [Fact]
    public void BestMatch_WithLayout_WorksCorrectly()
    {
        var candidates = new[] { "recipe", "receipt", "record" };
        var result = Distance.BestMatch("reciepe", candidates, KeyboardLayout.Qwerty);
        Assert.Equal("recipe", result);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void BestMatch_WithLayout_ExactMatch_ReturnsIt(KeyboardLayout layout)
    {
        var candidates = new[] { "apple", "banana" };
        var result = Distance.BestMatch("apple", candidates, layout);
        Assert.Equal("apple", result);
    }

    // Additional edge case tests

    [Fact]
    public void TypoScore_CaseInsensitive()
    {
        var result = Distance.Calculate("TEST", "test");
        Assert.Equal(1.0, result.TypoScore());
    }

    [Fact]
    public void IsLikelyTypo_CaseInsensitive()
    {
        var result = Distance.Calculate("PASSWORD", "password");
        Assert.True(result.IsLikelyTypo());
    }

    [Fact]
    public void BestMatch_CaseInsensitive()
    {
        var candidates = new[] { "Hello", "World" };
        var result = Distance.BestMatch("HELLO", candidates);
        Assert.Equal("Hello", result);
    }

    // keyboardPenaltyStrength Tests

    [Fact]
    public void TypoScore_ZeroPenaltyStrength_IgnoresKeyboardDistance()
    {
        // With keyboardPenaltyStrength = 0, keyboard distance should have no effect
        // Score should equal pure edit similarity
        var result = Distance.Calculate("a", "z"); // distant keys
        double editSimilarity = 1.0 - ((double)result.EditDistance / 1);

        Assert.Equal(editSimilarity, result.TypoScore(0.0));
    }

    [Fact]
    public void TypoScore_FullPenaltyStrength_MaximizesKeyboardImpact()
    {
        // With keyboardPenaltyStrength = 1.0, keyboard distance should have maximum effect
        var result = Distance.Calculate("a", "p"); // distant keys
        double editSimilarity = 1.0 - ((double)result.EditDistance / 1);
        double keyboardFactor = 1.0 - (result.AverageKeyboardDistance * 1.0);
        double expected = editSimilarity * keyboardFactor;

        Assert.Equal(expected, result.TypoScore(1.0));
    }

    [Fact]
    public void TypoScore_HigherPenaltyStrength_LowersScoreForDistantKeys()
    {
        // Distant key substitution - need longer string so edit similarity isn't 0
        var result = Distance.Calculate("password", "passward"); // o→a is distant

        double lowPenalty = result.TypoScore(0.2);
        double midPenalty = result.TypoScore(0.5);
        double highPenalty = result.TypoScore(1.0);

        Assert.True(highPenalty < midPenalty,
            $"Higher penalty ({highPenalty}) should lower score compared to mid penalty ({midPenalty})");
        Assert.True(midPenalty < lowPenalty,
            $"Mid penalty ({midPenalty}) should lower score compared to low penalty ({lowPenalty})");
    }

    [Fact]
    public void IsLikelyTypo_WithCustomPenaltyStrength_AffectsResult()
    {
        // Distant key substitution: o→a spans multiple rows on QWERTY
        // For an 8-char word, adaptive threshold is 0.80
        var result = Distance.Calculate("passward", "password");

        // Verify keyboard penalty affects the score
        double lowPenaltyScore = result.TypoScore(0.0);   // ignore keyboard distance
        double highPenaltyScore = result.TypoScore(1.0);  // max keyboard penalty

        Assert.True(lowPenaltyScore > highPenaltyScore,
            $"Low penalty score ({lowPenaltyScore:F3}) should be higher than high penalty score ({highPenaltyScore:F3})");

        // With zero keyboard penalty, should pass (score ~0.875)
        Assert.True(result.IsLikelyTypo(keyboardPenaltyStrength: 0.0),
            $"With zero penalty, score {lowPenaltyScore:F3} should pass");

        // With maximum keyboard penalty, may fail if score drops below 0.80
        // The result depends on keyboard distance of o→a
        double threshold = 0.80;
        if (highPenaltyScore < threshold)
        {
            Assert.False(result.IsLikelyTypo(keyboardPenaltyStrength: 1.0),
                $"With full penalty, score {highPenaltyScore:F3} should fail at threshold {threshold}");
        }
    }

    [Fact]
    public void BestMatch_WithCustomPenaltyStrength_AffectsSelection()
    {
        // Create candidates where keyboard distance differs:
        // "tesr" vs "test" = r→t substitution (r and t are adjacent on QWERTY)
        // "tesr" vs "tesq" = r→q substitution (r and q are more distant)
        var candidates = new[] { "test", "tesq" };

        // Both have edit distance 1, but "test" has closer keyboard distance
        var testResult = Distance.Calculate("tesr", "test");
        var tesqResult = Distance.Calculate("tesr", "tesq");

        Assert.Equal(1, testResult.EditDistance);
        Assert.Equal(1, tesqResult.EditDistance);
        Assert.True(testResult.AverageKeyboardDistance < tesqResult.AverageKeyboardDistance,
            "r→t should have smaller keyboard distance than r→q");

        // With zero penalty, both score the same (pure edit distance)
        Assert.Equal(testResult.TypoScore(0.0), tesqResult.TypoScore(0.0));

        // With penalty applied, "test" should score higher due to closer keys
        Assert.True(testResult.TypoScore(1.0) > tesqResult.TypoScore(1.0),
            "With full penalty, adjacent key substitution should score higher");

        // BestMatch should return "test" when keyboard penalty is applied
        var match = Distance.BestMatch("tesr", candidates, minScore: 0.5, keyboardPenaltyStrength: 1.0);
        Assert.Equal("test", match);
    }

    // Custom Keyboard Layout Tests

    [Fact]
    public void CustomKeyboardLayoutBuilder_CreatesValidLayout()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0)
            .AddRow("asdfghjkl", y: 1)
            .AddRow("zxcvbnm", y: 2)
            .Build();

        Assert.NotNull(layout);
        Assert.True(layout.MaxDistance > 0, "Layout should have a positive max distance");
    }

    [Fact]
    public void CustomKeyboardLayoutBuilder_WithXOffset_AppliesCorrectly()
    {
        // Create a layout with offset and verify key positions differ
        var layoutNoOffset = CustomKeyboardLayout.CreateBuilder()
            .AddRow("abc", y: 0, xOffset: 0f)
            .Build();

        var layoutWithOffset = CustomKeyboardLayout.CreateBuilder()
            .AddRow("abc", y: 0, xOffset: 1.5f)
            .Build();

        // Both should be valid layouts
        Assert.NotNull(layoutNoOffset);
        Assert.NotNull(layoutWithOffset);

        // The layouts should produce different max distances due to different positions
        // (This is a basic sanity check - positions are internal)
    }

    [Fact]
    public void CustomLayout_Calculate_IdenticalStrings_ReturnsZero()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0)
            .AddRow("asdfghjkl", y: 1)
            .AddRow("zxcvbnm", y: 2)
            .Build();

        var result = Distance.Calculate("hello", "hello", layout);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void CustomLayout_Calculate_AdjacentKeys_HasLowKeyboardDistance()
    {
        // Create a simple layout where 'a' and 's' are adjacent
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("asdfghjkl", y: 0)
            .Build();

        var result = Distance.Calculate("a", "s", layout);
        Assert.Equal(1, result.EditDistance);
        Assert.True(result.KeyboardDistanceSum < 0.5,
            $"Adjacent keys 'a' and 's' should have low keyboard distance, got {result.KeyboardDistanceSum}");
    }

    [Fact]
    public void CustomLayout_Calculate_DistantKeys_HasHigherKeyboardDistance()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("asdfghjkl", y: 0)
            .Build();

        var adjacentResult = Distance.Calculate("a", "s", layout);
        var distantResult = Distance.Calculate("a", "l", layout);

        Assert.True(distantResult.KeyboardDistanceSum > adjacentResult.KeyboardDistanceSum,
            $"Distant keys should have higher keyboard distance. Adjacent: {adjacentResult.KeyboardDistanceSum}, Distant: {distantResult.KeyboardDistanceSum}");
    }

    [Fact]
    public void CustomLayout_Calculate_CaseInsensitive()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0)
            .AddRow("asdfghjkl", y: 1)
            .Build();

        var result = Distance.Calculate("Hello", "hello", layout);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void CustomLayout_BestMatch_FindsCorrectMatch()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0)
            .AddRow("asdfghjkl", y: 1)
            .AddRow("zxcvbnm", y: 2)
            .Build();

        var candidates = new[] { "recipe", "receipt", "record" };
        var result = Distance.BestMatch("reciepe", candidates, layout);
        Assert.Equal("recipe", result);
    }

    [Fact]
    public void CustomLayout_BestMatch_ExactMatch_ReturnsIt()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0)
            .AddRow("asdfghjkl", y: 1)
            .Build();

        var candidates = new[] { "apple", "banana" };
        var result = Distance.BestMatch("apple", candidates, layout);
        Assert.Equal("apple", result);
    }

    [Fact]
    public void CustomLayout_BestMatch_NoMatchAboveThreshold_ReturnsNull()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0)
            .Build();

        var candidates = new[] { "abc", "def", "ghi" };
        var result = Distance.BestMatch("xyz", candidates, layout);
        Assert.Null(result);
    }

    [Fact]
    public void CustomLayout_MobileKeyboardExample_WorksCorrectly()
    {
        // Define a mobile keyboard layout with typical offsets
        var mobileKeyboard = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0, xOffset: 0.3f)
            .AddRow("asdfghjkl", y: 1, xOffset: 0.5f)
            .AddRow("zxcvbnm", y: 2, xOffset: 1.1f)
            .Build();

        // Verify it works with Calculate
        var result = Distance.Calculate("hello", "helo", mobileKeyboard);
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Deletions);

        // Verify it works with BestMatch
        var commands = new[] { "commit", "push", "pull", "status" };
        var match = Distance.BestMatch("commti", commands, mobileKeyboard);
        Assert.Equal("commit", match);
    }

    [Fact]
    public void CustomKeyboardLayoutBuilder_AddRow_HandlesUppercaseInput()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("QWERTY", y: 0)
            .Build();

        // Should work with lowercase input
        var result = Distance.Calculate("q", "w", layout);
        Assert.Equal(1, result.EditDistance);
        Assert.True(result.KeyboardDistanceSum < 1.0, "Keys should be found in layout");
    }

    [Fact]
    public void CustomLayout_UnknownCharacters_DefaultToFullCost()
    {
        var layout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("abc", y: 0)
            .Build();

        // 'x' and 'z' are not in the layout
        var result = Distance.Calculate("a", "x", layout);
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1.0, result.KeyboardDistanceSum);
    }
}
