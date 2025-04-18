﻿using System;

namespace Mdfry1.Scripts.Extensions;

public static class ArrayExtensions
{
    public static T Find<T>(this T[] array, Predicate<T> predicate)
    {
        foreach (var item in array)
            if (predicate(item))
                return item;
        return default;
    }
}

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string val)
    {
        return string.IsNullOrEmpty(val);
    }

    public static bool IsNullOrWhiteSpace(this string val)
    {
        return string.IsNullOrWhiteSpace(val);
    }

    public static bool Equals(this string val, string otherVal)
    {
        var retval = string.Equals(val, otherVal, StringComparison.OrdinalIgnoreCase);
        return retval;
    }
}