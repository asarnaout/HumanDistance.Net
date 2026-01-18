using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using HumanDistance;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

[MemoryDiagnoser]
[SimpleJob]
public class CalculateBenchmarks
{
    private readonly string _source;
    private readonly string _target;

    public CalculateBenchmarks()
    {
        _source = RandomText.Generate(64);
        _target = RandomText.Generate(64);
    }

    [Benchmark]
    public DistanceResult CalculateDefaultLayout()
    {
        return Distance.Calculate(_source, _target);
    }

    [Benchmark]
    public DistanceResult CalculateQwerty()
    {
        return Distance.Calculate(_source, _target, KeyboardLayout.Qwerty);
    }

    [Benchmark]
    public DistanceResult CalculateAzerty()
    {
        return Distance.Calculate(_source, _target, KeyboardLayout.Azerty);
    }

    [Benchmark]
    public DistanceResult CalculateQwertz()
    {
        return Distance.Calculate(_source, _target, KeyboardLayout.Qwertz);
    }
}

[MemoryDiagnoser]
[SimpleJob]
public class CalculateLengthBenchmarks
{
    [Params(4, 16, 64, 256)]
    public int Length { get; set; }

    private string _source = string.Empty;
    private string _target = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        _source = RandomText.Generate(Length);
        _target = RandomText.Generate(Length);
    }

    [Benchmark]
    public DistanceResult CalculateDefaultLayout()
    {
        return Distance.Calculate(_source, _target);
    }

    [Benchmark]
    public DistanceResult CalculateWithTransposition()
    {
        return Distance.Calculate(RandomText.InjectTransposition(_source), RandomText.InjectTransposition(_target));
    }
}

[MemoryDiagnoser]
[SimpleJob]
public class BestMatchBenchmarks
{
    [Params(10, 100, 1000)]
    public int CandidateCount { get; set; }

    private string _input = string.Empty;
    private string[] _candidates = Array.Empty<string>();

    [GlobalSetup]
    public void Setup()
    {
        _input = RandomText.Generate(12);
        _candidates = new string[CandidateCount];
        for (int i = 0; i < CandidateCount; i++)
        {
            _candidates[i] = RandomText.Generate(12);
        }

        if (CandidateCount > 0)
        {
            _candidates[CandidateCount / 2] = _input;
        }
    }

    [Benchmark]
    public string? BestMatchDefaultLayout()
    {
        return Distance.BestMatch(_input, _candidates);
    }

    [Benchmark]
    public string? BestMatchQwertyStrongPenalty()
    {
        return Distance.BestMatch(_input, _candidates, KeyboardLayout.Qwerty, minScore: 0.5, keyboardPenaltyStrength: 1.0);
    }
}

internal static class RandomText
{
    private static readonly char[] Alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly Random Random = new(8675309);

    public static string Generate(int length)
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = Alphabet[Random.Next(Alphabet.Length)];
        }

        return new string(chars);
    }

    public static string InjectTransposition(string input)
    {
        if (input.Length < 2)
        {
            return input;
        }

        var chars = input.ToCharArray();
        int index = Random.Next(0, chars.Length - 1);
        (chars[index], chars[index + 1]) = (chars[index + 1], chars[index]);
        return new string(chars);
    }
}
