namespace HumanDistance.Keyboards;

public abstract class KeyboardLayoutBase
{
    private readonly Dictionary<char, (float X, float Y)> _positions;

    public float MaxDistance { get; }

    protected KeyboardLayoutBase()
    {
        _positions = BuildPositions();
        MaxDistance = CalculateMaxDistance();
    }

    protected abstract Dictionary<char, (float X, float Y)> BuildPositions();

    public bool TryGetPosition(char c, out float x, out float y)
    {
        char lower = char.ToLowerInvariant(c);
        if (_positions.TryGetValue(lower, out var pos))
        {
            x = pos.X;
            y = pos.Y;
            return true;
        }

        x = 0;
        y = 0;
        return false;
    }

    private float CalculateMaxDistance()
    {
        float maxDist = 0;
        var keys = _positions.Values.ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            for (int j = i + 1; j < keys.Length; j++)
            {
                float dx = keys[i].X - keys[j].X;
                float dy = keys[i].Y - keys[j].Y;
                float dist = MathF.Sqrt(dx * dx + dy * dy);
                if (dist > maxDist)
                {
                    maxDist = dist;
                }
            }
        }

        return maxDist;
    }
}
