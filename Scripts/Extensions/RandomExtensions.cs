using System;

namespace Mdfry1.Scripts.Extensions;

public static class RandomExtensions
{
    public static int RandomInt(this Random random, int min, int max)
    {
        return random.Next(min, max);
    }

    public static double RandomDouble(this Random random, double minimum, double maximum)
    {
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    public static float RandomFloat(this Random random, float minimum, float maximum)
    {
        var val = (float)random.NextDouble();
        var retval = val * (maximum - minimum) + minimum;
        return retval;
    }
}