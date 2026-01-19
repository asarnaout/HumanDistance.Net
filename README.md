# HumanDistance

Smarter string distance that understands how humans actually make typos.

[![.NET](https://img.shields.io/badge/.NET-8%2B-512BD4)](https://dotnet.microsoft.com/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Traditional edit distance treats all substitutions the same. HumanDistance blends Damerau-Levenshtein with keyboard proximity so adjacent-key typos score as closer than distant ones. It also ships an adaptive IsLikelyTypo that just works for short and long words.

## Why It Beats Plain Edit Distance

Damerau-Levenshtein says these are both 1 edit away — but one is a likely keyboard typo and the other isn’t. HumanDistance captures that.

```csharp
// Adjacent vs distant substitution
var toSlop = Distance.Calculate("slip", "slop");  // i->o (adjacent keys)
var toSlap = Distance.Calculate("slip", "slap");  // i->a (distant keys)

toSlop.EditDistance;   // 1
toSlap.EditDistance;   // 1

toSlop.IsLikelyTypo(); // true  — adjacent substitution
toSlap.IsLikelyTypo(); // false — distant substitution

// What the user probably meant
Distance.BestMatch("slop", new[]{"slip","slap"}); // => "slip"
```

### More Examples

| Scenario                          | Pairs                                     | Plain DL | HumanDistance |
|-----------------------------------|-------------------------------------------|----------|---------------|
| Adjacent vs distant substitution  | slip→slop, slip→slap                      | both 1   | IsLikelyTypo: true / false; BestMatch("slop", ["slip","slap"]) = "slip" |
| Transposition vs distant sub      | form→from, form→farm                      | both 1   | IsLikelyTypo: true / false |
| Short-word transposition          | git→gti                                   | 1        | IsLikelyTypo: true (adaptive threshold) |

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
Length-aware boolean for “is this a likely typo?” — ideal for validation, CLI suggestions, and spell-check.

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

## Notes
- Case-insensitive by default
- Targets .NET 8.0 and 10.0
- For numeric comparisons or custom thresholds, use `TypoScore(keyboardPenaltyStrength)`

## License
MIT — see `LICENSE`.
