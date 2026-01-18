namespace HumanDistance.Tests;

public class CalculatorTests
{
    [Fact]
    public void Calculate_IdenticalStrings_ReturnsZero()
    {
        var result = Calculator.Calculate("hello", "hello");
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
        var result = Calculator.Calculate("", "");
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_EmptySource_ReturnsTargetLength()
    {
        var result = Calculator.Calculate("", "hello");
        Assert.Equal(5, result.EditDistance);
        Assert.Equal(5, result.Insertions);
        Assert.Equal(0, result.Deletions);
    }

    [Fact]
    public void Calculate_EmptyTarget_ReturnsSourceLength()
    {
        var result = Calculator.Calculate("hello", "");
        Assert.Equal(5, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(5, result.Deletions);
    }

    [Fact]
    public void Calculate_SingleInsertion_ReturnsOne()
    {
        var result = Calculator.Calculate("hell", "hello");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Insertions);
        Assert.Equal(0, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
    }

    [Fact]
    public void Calculate_SingleDeletion_ReturnsOne()
    {
        var result = Calculator.Calculate("hello", "hell");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(0, result.Insertions);
        Assert.Equal(1, result.Deletions);
        Assert.Equal(0, result.Substitutions);
        Assert.Equal(0, result.Transpositions);
    }

    [Fact]
    public void Calculate_SingleSubstitution_ReturnsOne()
    {
        var result = Calculator.Calculate("hello", "hallo");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.True(result.KeyboardDistanceSum > 0.0, "Keyboard distance should be positive for substitution");
        Assert.True(result.KeyboardDistanceSum <= 1.0, "Single substitution keyboard distance should be at most 1.0");
    }

    [Fact]
    public void Calculate_Transposition_ReturnsOne()
    {
        var result = Calculator.Calculate("hello", "hlelo");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Transpositions);
        Assert.Equal(0, result.Substitutions);
    }

    [Fact]
    public void Calculate_CaseInsensitive()
    {
        var result = Calculator.Calculate("Hello", "hello");
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_AdjacentKeys_HasLowKeyboardDistance()
    {
        var result = Calculator.Calculate("a", "s");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.True(result.KeyboardDistanceSum < 0.5,
            $"Adjacent keys 'a' and 's' should have low keyboard distance, got {result.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_DistantKeys_HasHigherKeyboardDistance()
    {
        var adjacentResult = Calculator.Calculate("a", "s");
        var distantResult = Calculator.Calculate("a", "p");
        Assert.True(distantResult.KeyboardDistanceSum > adjacentResult.KeyboardDistanceSum,
            $"Distant keys should have higher keyboard distance. Adjacent: {adjacentResult.KeyboardDistanceSum}, Distant: {distantResult.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_UnknownCharacters_DefaultToFullCost()
    {
        var result = Calculator.Calculate("a", "α");
        Assert.Equal(1, result.EditDistance);
        Assert.Equal(1, result.Substitutions);
        Assert.Equal(1.0, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_DefaultsToQwerty()
    {
        var result = Calculator.Calculate("q", "a");
        var qwertyResult = Calculator.Calculate("q", "a", KeyboardLayout.Qwerty);
        Assert.Equal(qwertyResult.EditDistance, result.EditDistance);
        Assert.Equal(qwertyResult.KeyboardDistanceSum, result.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_WithLayout_IdenticalStrings_ReturnsZero()
    {
        var result = Calculator.Calculate("hello", "hello", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_AdjacentKeys_HasLowKeyboardDistance()
    {
        var result = Calculator.Calculate("a", "s", KeyboardLayout.Qwerty);
        Assert.Equal(1, result.EditDistance);
        Assert.True(result.KeyboardDistanceSum < 0.5,
            $"Adjacent keys 'a' and 's' should have low keyboard distance, got {result.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_WithLayout_DistantKeys_HasHigherKeyboardDistance()
    {
        var adjacentResult = Calculator.Calculate("a", "s", KeyboardLayout.Qwerty);
        var distantResult = Calculator.Calculate("a", "p", KeyboardLayout.Qwerty);
        Assert.True(distantResult.KeyboardDistanceSum > adjacentResult.KeyboardDistanceSum,
            $"Distant keys should have higher keyboard distance. Adjacent: {adjacentResult.KeyboardDistanceSum}, Distant: {distantResult.KeyboardDistanceSum}");
    }

    [Fact]
    public void Calculate_WithLayout_QwertyLayout_WorksCorrectly()
    {
        var result = Calculator.Calculate("qwerty", "qwerty", KeyboardLayout.Qwerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_AzertyLayout_WorksCorrectly()
    {
        var result = Calculator.Calculate("azerty", "azerty", KeyboardLayout.Azerty);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_QwertzLayout_WorksCorrectly()
    {
        var result = Calculator.Calculate("qwertz", "qwertz", KeyboardLayout.Qwertz);
        Assert.Equal(0, result.EditDistance);
    }

    [Fact]
    public void Calculate_WithLayout_CaseInsensitive()
    {
        var result = Calculator.Calculate("Hello", "hello", KeyboardLayout.Qwerty);
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
        var result = Calculator.Calculate(source, target);
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
        var adjacentResult = Calculator.Calculate(source1, adjacentTarget, layout);
        var distantResult = Calculator.Calculate(source2, distantTarget, layout);
        Assert.True(adjacentResult.KeyboardDistanceSum < distantResult.KeyboardDistanceSum,
            $"Layout {layout}: Adjacent keys ({source1}->{adjacentTarget}={adjacentResult.KeyboardDistanceSum:F3}) should have lower keyboard distance than distant keys ({source2}->{distantTarget}={distantResult.KeyboardDistanceSum:F3})");
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void Calculate_WithLayout_UnknownCharacters_DefaultToFullCost(KeyboardLayout layout)
    {
        var result = Calculator.Calculate("a", "α", layout);
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
        var result = Calculator.Calculate(source, target, layout);
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
        var result = Calculator.Calculate(source, target, layout);
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
        var result = Calculator.Calculate(source, target, layout);
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
        var result = Calculator.Calculate(source, target, layout);
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
        var result = Calculator.Calculate(source, target);
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
        var insertResult = Calculator.Calculate("abc", "abcde");
        Assert.Equal(0, insertResult.Substitutions);
        Assert.Equal(0.0, insertResult.KeyboardDistanceSum);

        // Pure deletions
        var deleteResult = Calculator.Calculate("hello", "hel");
        Assert.Equal(0, deleteResult.Substitutions);
        Assert.Equal(0.0, deleteResult.KeyboardDistanceSum);

        // Pure transposition
        var transposeResult = Calculator.Calculate("ab", "ba");
        Assert.Equal(0, transposeResult.Substitutions);
        Assert.Equal(0.0, transposeResult.KeyboardDistanceSum);

        // Identical strings
        var identicalResult = Calculator.Calculate("test", "test");
        Assert.Equal(0, identicalResult.Substitutions);
        Assert.Equal(0.0, identicalResult.KeyboardDistanceSum);
    }

    [Fact]
    public void Calculate_AverageKeyboardDistance_CalculatedCorrectly()
    {
        // Single substitution
        var singleResult = Calculator.Calculate("a", "s");
        Assert.Equal(singleResult.KeyboardDistanceSum, singleResult.AverageKeyboardDistance);

        // Multiple substitutions
        var multiResult = Calculator.Calculate("abc", "xyz");
        Assert.Equal(3, multiResult.Substitutions);
        Assert.Equal(multiResult.KeyboardDistanceSum / 3, multiResult.AverageKeyboardDistance);
    }

    [Fact]
    public void Calculate_NoSubstitutions_AverageKeyboardDistanceIsZero()
    {
        var result = Calculator.Calculate("abc", "abcde"); // Pure insertions
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
        var result = Calculator.Calculate(source, target);
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
        var azResult = Calculator.Calculate("a", "z", KeyboardLayout.Qwerty);
        var bxResult = Calculator.Calculate("b", "x", KeyboardLayout.Qwerty);

        // Calculate combined
        var combinedResult = Calculator.Calculate("ab", "zx", KeyboardLayout.Qwerty);

        Assert.Equal(2, combinedResult.Substitutions);
        Assert.Equal(azResult.KeyboardDistanceSum + bxResult.KeyboardDistanceSum,
            combinedResult.KeyboardDistanceSum, precision: 10);
    }

    // TypoScore Tests

    [Fact]
    public void TypoScore_IdenticalStrings_ReturnsOne()
    {
        var result = Calculator.Calculate("password", "password");
        Assert.Equal(1.0, result.TypoScore());
    }

    [Fact]
    public void TypoScore_BothEmpty_ReturnsOne()
    {
        var result = Calculator.Calculate("", "");
        Assert.Equal(1.0, result.TypoScore());
    }

    [Fact]
    public void TypoScore_EmptyVsNonEmpty_ReturnsZero()
    {
        var result = Calculator.Calculate("", "test");
        Assert.Equal(0.0, result.TypoScore());
    }

    [Fact]
    public void TypoScore_SingleTransposition_HighScore()
    {
        // "passwrod" vs "password" - single transposition (o and r swapped)
        var result = Calculator.Calculate("passwrod", "password");
        Assert.True(result.TypoScore() > 0.8, $"Single transposition should have high score, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_SingleAdjacentKeySubstitution_HighScore()
    {
        // "passwprd" vs "password" - o→p is a truly adjacent key typo on QWERTY
        var result = Calculator.Calculate("passwprd", "password");
        Assert.True(result.TypoScore() > 0.8, $"Adjacent key substitution should have high score, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_SingleDistantKeySubstitution_LowerThanAdjacent()
    {
        // Compare adjacent vs distant substitution on QWERTY
        // 'o' is adjacent to 'i' and 'p', but far from 'a'
        var adjacentResult = Calculator.Calculate("passwprd", "password"); // o→p (adjacent)
        var distantResult = Calculator.Calculate("passward", "password");  // o→a (distant - different row)

        Assert.True(distantResult.TypoScore() < adjacentResult.TypoScore(),
            $"Distant key substitution ({distantResult.TypoScore()}) should score lower than adjacent ({adjacentResult.TypoScore()})");
    }

    [Fact]
    public void TypoScore_CompletelyDifferent_LowScore()
    {
        var result = Calculator.Calculate("qwertyui", "password");
        Assert.True(result.TypoScore() < 0.3, $"Completely different strings should have low score, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_ScoreNeverNegative()
    {
        // Even with maximum penalties, score should not go negative
        var result = Calculator.Calculate("aaaa", "zzzz");
        Assert.True(result.TypoScore() >= 0.0, $"Score should never be negative, got {result.TypoScore()}");
    }

    [Fact]
    public void TypoScore_WithLayout_WorksCorrectly()
    {
        var qwertyResult = Calculator.Calculate("a", "s", KeyboardLayout.Qwerty);
        var azertyResult = Calculator.Calculate("a", "s", KeyboardLayout.Azerty);

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
        var result = Calculator.Calculate("test", "test", layout);
        Assert.Equal(1.0, result.TypoScore());
    }

    // IsLikelyTypo Tests

    [Fact]
    public void IsLikelyTypo_CommonTypo_ReturnsTrue()
    {
        // "teh" is a common typo for "the"
        var result = Calculator.Calculate("teh", "the");
        Assert.True(result.IsLikelyTypo(threshold: 0.6));
    }

    [Fact]
    public void IsLikelyTypo_CompletelyDifferent_ReturnsFalse()
    {
        var result = Calculator.Calculate("hello", "world");
        Assert.False(result.IsLikelyTypo(threshold: 0.8));
    }

    [Fact]
    public void IsLikelyTypo_IdenticalStrings_ReturnsTrue()
    {
        var result = Calculator.Calculate("test", "test");
        Assert.True(result.IsLikelyTypo());
    }

    [Fact]
    public void IsLikelyTypo_DefaultThreshold_IsPractical()
    {
        // Single character transposition in 8-char word should pass default 0.8 threshold
        var result = Calculator.Calculate("passwrod", "password");
        Assert.True(result.IsLikelyTypo(), "Single transposition should be detected as typo with default threshold");
    }

    [Fact]
    public void IsLikelyTypo_WithLayout_WorksCorrectly()
    {
        var result = Calculator.Calculate("teh", "the", KeyboardLayout.Qwerty);
        Assert.True(result.IsLikelyTypo(threshold: 0.6));
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void IsLikelyTypo_WithLayout_IdenticalStrings_ReturnsTrue(KeyboardLayout layout)
    {
        var result = Calculator.Calculate("test", "test", layout);
        Assert.True(result.IsLikelyTypo());
    }

    // BestMatch Tests

    [Fact]
    public void BestMatch_FindsCorrectMatch()
    {
        var candidates = new[] { "recipe", "receipt", "record" };
        var result = Calculator.BestMatch("reciepe", candidates);
        Assert.Equal("recipe", result);
    }

    [Fact]
    public void BestMatch_NoMatchAboveThreshold_ReturnsNull()
    {
        var candidates = new[] { "abc", "def", "ghi" };
        var result = Calculator.BestMatch("xyz", candidates);
        Assert.Null(result);
    }

    [Fact]
    public void BestMatch_EmptyCandidates_ReturnsNull()
    {
        var candidates = Array.Empty<string>();
        var result = Calculator.BestMatch("test", candidates);
        Assert.Null(result);
    }

    [Fact]
    public void BestMatch_ExactMatchInCandidates_ReturnsIt()
    {
        var candidates = new[] { "apple", "banana", "cherry" };
        var result = Calculator.BestMatch("banana", candidates);
        Assert.Equal("banana", result);
    }

    [Fact]
    public void BestMatch_WithCustomMinScore_Respected()
    {
        var candidates = new[] { "test", "tast", "tost" };

        // With low minScore, should find a match
        var lowThreshold = Calculator.BestMatch("txst", candidates, minScore: 0.3);
        Assert.NotNull(lowThreshold);

        // With very high minScore, might not find a match
        var highThreshold = Calculator.BestMatch("txst", candidates, minScore: 0.99);
        Assert.Null(highThreshold);
    }

    [Fact]
    public void BestMatch_MultipleSimilarCandidates_ReturnsBest()
    {
        var candidates = new[] { "testing", "tasting", "tosting" };
        var result = Calculator.BestMatch("testin", candidates);
        Assert.Equal("testing", result);
    }

    [Fact]
    public void BestMatch_WithLayout_WorksCorrectly()
    {
        var candidates = new[] { "recipe", "receipt", "record" };
        var result = Calculator.BestMatch("reciepe", candidates, KeyboardLayout.Qwerty);
        Assert.Equal("recipe", result);
    }

    [Theory]
    [InlineData(KeyboardLayout.Qwerty)]
    [InlineData(KeyboardLayout.Azerty)]
    [InlineData(KeyboardLayout.Qwertz)]
    public void BestMatch_WithLayout_ExactMatch_ReturnsIt(KeyboardLayout layout)
    {
        var candidates = new[] { "apple", "banana" };
        var result = Calculator.BestMatch("apple", candidates, layout);
        Assert.Equal("apple", result);
    }

    // Additional edge case tests

    [Fact]
    public void TypoScore_CaseInsensitive()
    {
        var result = Calculator.Calculate("TEST", "test");
        Assert.Equal(1.0, result.TypoScore());
    }

    [Fact]
    public void IsLikelyTypo_CaseInsensitive()
    {
        var result = Calculator.Calculate("PASSWORD", "password");
        Assert.True(result.IsLikelyTypo());
    }

    [Fact]
    public void BestMatch_CaseInsensitive()
    {
        var candidates = new[] { "Hello", "World" };
        var result = Calculator.BestMatch("HELLO", candidates);
        Assert.Equal("Hello", result);
    }

    // keyboardPenaltyStrength Tests

    [Fact]
    public void TypoScore_ZeroPenaltyStrength_IgnoresKeyboardDistance()
    {
        // With keyboardPenaltyStrength = 0, keyboard distance should have no effect
        // Score should equal pure edit similarity
        var result = Calculator.Calculate("a", "z"); // distant keys
        double editSimilarity = 1.0 - ((double)result.EditDistance / 1);

        Assert.Equal(editSimilarity, result.TypoScore(0.0));
    }

    [Fact]
    public void TypoScore_FullPenaltyStrength_MaximizesKeyboardImpact()
    {
        // With keyboardPenaltyStrength = 1.0, keyboard distance should have maximum effect
        var result = Calculator.Calculate("a", "p"); // distant keys
        double editSimilarity = 1.0 - ((double)result.EditDistance / 1);
        double keyboardFactor = 1.0 - (result.AverageKeyboardDistance * 1.0);
        double expected = editSimilarity * keyboardFactor;

        Assert.Equal(expected, result.TypoScore(1.0));
    }

    [Fact]
    public void TypoScore_HigherPenaltyStrength_LowersScoreForDistantKeys()
    {
        // Distant key substitution - need longer string so edit similarity isn't 0
        var result = Calculator.Calculate("password", "passward"); // o→a is distant

        double lowPenalty = result.TypoScore(0.2);
        double midPenalty = result.TypoScore(0.5);
        double highPenalty = result.TypoScore(1.0);

        Assert.True(highPenalty < midPenalty,
            $"Higher penalty ({highPenalty}) should lower score compared to mid penalty ({midPenalty})");
        Assert.True(midPenalty < lowPenalty,
            $"Mid penalty ({midPenalty}) should lower score compared to low penalty ({lowPenalty})");
    }

    [Fact]
    public void IsLikelyTypo_WithCustomPenaltyStrength_CanFlipResult()
    {
        // Distant key substitution: o→a spans multiple rows on QWERTY
        var result = Calculator.Calculate("passward", "password");

        // Calculate scores to find a threshold that demonstrates the effect
        double lowPenaltyScore = result.TypoScore(0.0);
        double highPenaltyScore = result.TypoScore(1.0);

        // Use a threshold between the two scores
        double threshold = (lowPenaltyScore + highPenaltyScore) / 2;

        // With zero penalty, score is above threshold (likely typo)
        Assert.True(result.IsLikelyTypo(threshold, keyboardPenaltyStrength: 0.0),
            $"With zero penalty, score {lowPenaltyScore} should be >= threshold {threshold}");

        // With full penalty, score is below threshold (not likely typo)
        Assert.False(result.IsLikelyTypo(threshold, keyboardPenaltyStrength: 1.0),
            $"With full penalty, score {highPenaltyScore} should be < threshold {threshold}");
    }

    [Fact]
    public void BestMatch_WithCustomPenaltyStrength_AffectsSelection()
    {
        // Create candidates where keyboard distance differs:
        // "tesr" vs "test" = r→t substitution (r and t are adjacent on QWERTY)
        // "tesr" vs "tesq" = r→q substitution (r and q are more distant)
        var candidates = new[] { "test", "tesq" };

        // Both have edit distance 1, but "test" has closer keyboard distance
        var testResult = Calculator.Calculate("tesr", "test");
        var tesqResult = Calculator.Calculate("tesr", "tesq");

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
        var match = Calculator.BestMatch("tesr", candidates, minScore: 0.5, keyboardPenaltyStrength: 1.0);
        Assert.Equal("test", match);
    }
}
