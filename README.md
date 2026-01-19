# HumanDistance

Smarter string distance that understands how humans actually make typos.

[![.NET](https://img.shields.io/badge/.NET-8%2B-512BD4)](https://dotnet.microsoft.com/) [![NuGet](https://img.shields.io/nuget/v/HumanDistance.svg)](https://www.nuget.org/packages/HumanDistance) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Traditional edit distance treats all substitutions the same. HumanDistance blends Damerau-Levenshtein with keyboard proximity so adjacent-key typos score as closer than distant ones. It also ships an adaptive IsLikelyTypo that just works for short and long words.

## Why It Beats Plain Edit Distance

Damerau-Levenshtein says these are both 1 edit away — but one is a likely keyboard typo and the other isn’t. HumanDistance captures that.

```csharp
// Adjacent vs distant substitution
var toSlop = Distance.Calculate("slip", "slop");  // i->o (adjacent keys)
var toSlap = Distance.Calculate("slip", "slap");  // i->a (distant keys)

// Edit Distance alone does not give information about the likelihood of a typo
toSlop.EditDistance;   // 1
toSlap.EditDistance;   // 1

toSlop.IsLikelyTypo(); // true  — adjacent substitution
toSlap.IsLikelyTypo(); // false — distant substitution

// What the user probably meant
Distance.BestMatch("slop", new[]{"slip","slap"}); // => "slip"
```

### More Examples

| Source | Target | IsLikelyTypo | Plain Edit Distance |
|--------|--------|--------------|---------------------|
| jello  | hello  | true         | 1                   |
| cello  | hello  | false        | 1                   |
| from   | form   | true         | 1                   |
| farm   | form   | false        | 1                   |
| gti    | git    | true         | 1                   |

## Install

```bash
dotnet add package HumanDistance
```

## Quick Start

```csharp
using HumanDistance;

// Is this probably a typo?
var result = Distance.Calculate("stauts", "status");
if (result.IsLikelyTypo())
{
    Console.WriteLine("Probably a typo");
}

// Find what the user meant
string[] commands = ["status", "stage", "stash", "switch"];
string? match = Distance.BestMatch("stauts", commands);
// => "status"
```

## Why HumanDistance
- Keyboard-aware: adjacent key substitutions score closer than distant keys
- Adaptive typo detection: `IsLikelyTypo()` uses length-aware thresholds that catch single-char typos in short words
- Practical: `BestMatch()` picks the closest candidate from any list
- Detailed metrics: edit distance, ops breakdown, and keyboard distance
- Fast and allocation-friendly: uses `ReadOnlySpan<char>` and case-insensitive comparison
- Multiple layouts: QWERTY (default), AZERTY, QWERTZ; custom layouts supported

## API Essentials

### 1) IsLikelyTypo (centerpiece)
Length-aware boolean for "is this a likely typo?" — ideal for validation, CLI suggestions, and spell-check.

```csharp
var a = Distance.Calculate("git", "gti");  // transposition
Console.WriteLine(a.IsLikelyTypo());        // true (short words handled well)

var b = Distance.Calculate("pitting", "potting");  // i->o (adjacent)
var c = Distance.Calculate("pitting", "patting");  // i->a (distant)
Console.WriteLine(b.IsLikelyTypo());        // true
Console.WriteLine(c.IsLikelyTypo());        // false
```

Tip: prefer `IsLikelyTypo()` for decisions; use `TypoScore()` when you need a numeric score or custom thresholds.

### 2) BestMatch
Find the best candidate from a collection. Optionally tune keyboard penalty strength and a minimum score.

```csharp
string[] candidates = ["apple", "banana", "cherry"];
string? match = Distance.BestMatch("banan", candidates, minScore: 0.6);
```

### 3) Calculate (rich metrics)
Get standard Damerau-Levenshtein distance plus keyboard-aware substitution metrics.

```csharp
DistanceResult r = Distance.Calculate("hello", "hlelo");
// r.EditDistance == 1 (transposition)
// r.Substitutions, r.Transpositions, r.Insertions, r.Deletions
// r.AverageKeyboardDistance (0.0–1.0), r.TypoScore(), r.IsLikelyTypo()
```

## Keyboard Layouts

```csharp
// Built-in
Distance.Calculate("source", "target", KeyboardLayout.Qwerty);
Distance.Calculate("source", "target", KeyboardLayout.Azerty);
Distance.Calculate("source", "target", KeyboardLayout.Qwertz);

// Custom (e.g., mobile keyboard offsets)
using HumanDistance.Keyboards;
var mobile = CustomKeyboardLayout.CreateBuilder()
    .AddRow("qwertyuiop", y: 0, xOffset: 0.3f)
    .AddRow("asdfghjkl",  y: 1, xOffset: 0.5f)
    .AddRow("zxcvbnm",    y: 2, xOffset: 1.1f)
    .Build();
var res = Distance.Calculate("hello", "helo", mobile);
```

## Performance

BenchmarkDotNet (13th‑gen i7, .NET 10, Release)

- Calculate vs length (Default layout):

| Length | Mean   | Allocated |
|-------:|-------:|----------:|
| 4      | 142 ns |    216 B  |
| 16     | 1.20 µs|   1.5 KB  |
| 64     | 15.14 µs|  20.7 KB |
| 256    | 359.3 µs| 330.4 KB |

- Calculate (64‑char random strings) by layout:

| Layout   | Mean    | Allocated |
|----------|--------:|----------:|
| Default  | 14.90 µs|  20.72 KB |
| Qwerty   | 15.49 µs|  20.72 KB |
| Azerty   | 16.77 µs|  25.67 KB |
| Qwertz   | 16.99 µs|  25.68 KB |
| Custom   | 15.42 µs|  20.72 KB |

- BestMatch (64‑char inputs) scales with candidate count:

| Candidates | Mean     | Allocated |
|-----------:|---------:|----------:|
| 10         | 7.28 µs  |   9.14 KB |
| 100        | 70.36 µs |  91.41 KB |
| 1000       | 785.73 µs| 914.06 KB |

Notes: results vary by hardware and inputs; numbers above are representative on the listed machine.

## Notes
- Case-insensitive by default
- Targets .NET 8.0 and 10.0
- For numeric comparisons or custom thresholds, use `TypoScore(keyboardPenaltyStrength)`

## License
MIT — see `LICENSE`.
