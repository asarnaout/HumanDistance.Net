using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using HumanDistance.Keyboards;

namespace HumanDistance.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}

[MemoryDiagnoser]
[SimpleJob]
public class CalculateBenchmarks
{
    private string _source = string.Empty;
    private string _target = string.Empty;
    private CustomKeyboardLayout _customLayout = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _source = RandomText.Generate(64, random);
        _target = RandomText.Generate(64, random);
        _customLayout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0, xOffset: 0.3f)
            .AddRow("asdfghjkl", y: 1, xOffset: 0.5f)
            .AddRow("zxcvbnm", y: 2, xOffset: 1.1f)
            .Build();
    }

    [Benchmark(Baseline = true)]
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

    [Benchmark]
    public DistanceResult CalculateCustomLayout()
    {
        return Distance.Calculate(_source, _target, _customLayout);
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
    private string _sourceTransposed = string.Empty;
    private string _targetTransposed = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _source = RandomText.Generate(Length, random);
        _target = RandomText.Generate(Length, random);
        _sourceTransposed = RandomText.InjectTransposition(_source, random);
        _targetTransposed = RandomText.InjectTransposition(_target, random);
    }

    [Benchmark]
    public DistanceResult CalculateDefaultLayout()
    {
        return Distance.Calculate(_source, _target);
    }

    [Benchmark]
    public DistanceResult CalculateWithTransposition()
    {
        return Distance.Calculate(_sourceTransposed, _targetTransposed);
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
    private CustomKeyboardLayout _customLayout = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _input = RandomText.Generate(12, random);
        _candidates = new string[CandidateCount];
        for (int i = 0; i < CandidateCount; i++)
        {
            _candidates[i] = RandomText.Generate(12, random);
        }

        if (CandidateCount > 0)
        {
            _candidates[CandidateCount / 2] = _input;
        }

        _customLayout = CustomKeyboardLayout.CreateBuilder()
            .AddRow("qwertyuiop", y: 0, xOffset: 0.3f)
            .AddRow("asdfghjkl", y: 1, xOffset: 0.5f)
            .AddRow("zxcvbnm", y: 2, xOffset: 1.1f)
            .Build();
    }

    [Benchmark(Baseline = true)]
    public string? BestMatchDefaultLayout()
    {
        return Distance.BestMatch(_input, _candidates);
    }

    [Benchmark]
    public string? BestMatchQwertyStrongPenalty()
    {
        return Distance.BestMatch(_input, _candidates, KeyboardLayout.Qwerty, minScore: 0.5, keyboardPenaltyStrength: 1.0);
    }

    [Benchmark]
    public string? BestMatchCustomLayout()
    {
        return Distance.BestMatch(_input, _candidates, _customLayout);
    }
}

internal static class RandomText
{
    private static readonly char[] Alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    public static string Generate(int length, Random random)
    {
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(chars);
    }

    public static string InjectTransposition(string input, Random random)
    {
        if (input.Length < 2)
        {
            return input;
        }

        var chars = input.ToCharArray();
        int index = random.Next(0, chars.Length - 1);
        (chars[index], chars[index + 1]) = (chars[index + 1], chars[index]);
        return new string(chars);
    }
}
