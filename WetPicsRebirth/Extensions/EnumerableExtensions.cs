using System.Collections.Generic;

namespace WetPicsRebirth.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> ToEnumerable<T>(this T item) => new[] { item };
}