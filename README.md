# HumanDistance

**A smarter string distance library that understands how humans actually make typos.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

Traditional edit distance treats all character substitutions equally—it can't tell likely typos from unlikely ones.

HumanDistance combines **Damerau-Levenshtein edit distance** with **keyboard proximity awareness** to identify typos the way humans actually make them.

```csharp
// Both "potting" and "patting" are 1 edit away from "pitting"
// Traditional Levenshtein sees no difference — but one is a likely typo

var toPotting = Distance.Calculate("pitting", "potting");  // i → o (adjacent keys)
var toPatting = Distance.Calculate("pitting", "patting");  // i → a (distant keys)

toPotting.EditDistance;    // 1
toPatting.EditDistance;    // 1

toPotting.IsLikelyTypo();  // true — 'o' is right next to 'i'
toPatting.IsLikelyTypo();  // false — 'a' is too far from 'i'

// Find what the user probably meant
Distance.BestMatch("potting", ["pitting", "patting"]);  // Returns "pitting"
```

## Features

- **Keyboard-Aware Scoring** — Adjacent key substitutions score higher than distant ones
- **Multiple Keyboard Layouts** — QWERTY, AZERTY, and QWERTZ support
- **Detailed Metrics** — Get counts for insertions, deletions, substitutions, and transpositions
- **Best Match Finding** — Find the closest match from a list of candidates
- **Configurable Sensitivity** — Tune how much keyboard distance affects scoring
- **High Performance** — Uses `ReadOnlySpan<char>` for zero-allocation string handling
- **Case Insensitive** — Comparisons ignore case by default

## Installation

```bash
dotnet add package HumanDistance
```

## Quick Start

```csharp
using HumanDistance;

// Basic distance calculation
var result = Distance.Calculate("hello", "helo");
Console.WriteLine(result.EditDistance);    // 1
Console.WriteLine(result.TypoScore());     // 0.8

// Check if strings are likely typos of each other
if (result.IsLikelyTypo())
{
    Console.WriteLine("Probably a typo!");
}

// Find best match from candidates
string[] commands = ["status", "stage", "stash", "switch"];
string? match = Distance.BestMatch("stauts", commands);
Console.WriteLine(match);  // "status"
```

## API Reference

### Distance.Calculate

Calculates the edit distance and keyboard metrics between two strings.

```csharp
// Using default QWERTY layout
DistanceResult result = Distance.Calculate("source", "target");

// Using a specific keyboard layout
DistanceResult result = Distance.Calculate("source", "target", KeyboardLayout.Azerty);
```

### Distance.BestMatch

Finds the best matching string from a collection of candidates.

```csharp
string[] candidates = ["apple", "banana", "cherry"];

// Basic usage
string? match = Distance.BestMatch("banan", candidates);

// With custom minimum score threshold
string? match = Distance.BestMatch("banan", candidates, minScore: 0.7);

// With custom keyboard penalty strength
string? match = Distance.BestMatch("banan", candidates, minScore: 0.5, keyboardPenaltyStrength: 0.8);

// With specific keyboard layout
string? match = Distance.BestMatch("banan", candidates, KeyboardLayout.Qwertz, minScore: 0.5);
```

### DistanceResult

The result struct containing all distance metrics.

| Property | Type | Description |
|----------|------|-------------|
| `EditDistance` | `int` | Standard Damerau-Levenshtein distance |
| `Insertions` | `int` | Number of character insertions |
| `Deletions` | `int` | Number of character deletions |
| `Substitutions` | `int` | Number of character substitutions |
| `Transpositions` | `int` | Number of adjacent character swaps |
| `KeyboardDistanceSum` | `double` | Sum of normalized keyboard distances for substitutions |
| `AverageKeyboardDistance` | `double` | Average keyboard distance per substitution (0.0–1.0) |

| Method | Description |
|--------|-------------|
| `TypoScore(keyboardPenaltyStrength)` | Similarity score from 0.0 to 1.0, accounting for keyboard proximity |
| `IsLikelyTypo(threshold, keyboardPenaltyStrength)` | Returns `true` if `TypoScore` meets the threshold |

### Keyboard Layouts

```csharp
KeyboardLayout.Qwerty  // Default - US/UK standard
KeyboardLayout.Azerty  // French standard
KeyboardLayout.Qwertz  // German/Central European standard
```

## Understanding the Scores

### TypoScore

A similarity score from 0.0 (completely different) to 1.0 (identical) that factors in both edit distance and keyboard proximity.

```csharp
var result = Distance.Calculate("test", "test");
result.TypoScore();  // 1.0 - identical

var result = Distance.Calculate("test", "tset");
result.TypoScore();  // 0.75 - transposition (1 edit in 4 chars)

var result = Distance.Calculate("test", "tesr");
result.TypoScore();  // 0.72 - t→r substitution with adjacent keys
```

### Keyboard Penalty Strength

Control how much keyboard distance affects the final score:

```csharp
var result = Distance.Calculate("hello", "jello");  // h → j (adjacent keys)

result.TypoScore(0.0);  // 0.80 - ignore keyboard, pure edit distance
result.TypoScore(0.5);  // 0.77 - default, moderate keyboard penalty
result.TypoScore(1.0);  // 0.74 - maximum keyboard penalty
```

- `0.0` — Keyboard distance ignored; pure edit-distance-based similarity
- `0.5` — Default; balanced consideration of edit distance and keyboard proximity
- `1.0` — Maximum penalty for distant key substitutions

## Use Cases

### Spell Checking
```csharp
string[] dictionary = LoadDictionary();
string? suggestion = Distance.BestMatch(userInput, dictionary, minScore: 0.7);
```

### Command Line Interfaces
```csharp
string[] commands = ["commit", "checkout", "cherry-pick", "clone"];
string? didYouMean = Distance.BestMatch(invalidCommand, commands);
if (didYouMean != null)
    Console.WriteLine($"Did you mean '{didYouMean}'?");
```

### Form Validation
```csharp
var result = Distance.Calculate(confirmedEmail, originalEmail);
if (!result.IsLikelyTypo(threshold: 0.95))
    ShowWarning("The emails don't match. Did you make a typo?");
```

### Fuzzy Search
```csharp
var matches = products
    .Select(p => (Product: p, Score: Distance.Calculate(query, p.Name).TypoScore()))
    .Where(x => x.Score > 0.6)
    .OrderByDescending(x => x.Score);
```

## How It Works

HumanDistance uses the **Damerau-Levenshtein algorithm** to compute edit distance, which counts:
- **Insertions** — Characters added
- **Deletions** — Characters removed
- **Substitutions** — Characters replaced
- **Transpositions** — Adjacent characters swapped

For each substitution, it also calculates the **physical distance** between the two keys on the keyboard. This distance is normalized (0.0 for adjacent keys, 1.0 for maximum distance) and factored into the final similarity score.

The result: typos that humans commonly make (hitting an adjacent key) are scored as more similar than random character replacements.

## Benchmarks

Run the BenchmarkDotNet suite with:

```bash
dotnet run -c Release --project HumanDistance.Benchmarks
```

### Results

Benchmark results are recorded here after running the suite:

```
Pending — run the benchmark command above and paste the summary output here.
```

## License

MIT License. See [LICENSE](LICENSE) for details.
