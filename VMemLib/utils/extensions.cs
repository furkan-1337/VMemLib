namespace VMemLib.utils;

public static class extensions
{
    public static T Assert<T>(this T value, Func<T, bool> condition, string message)
    {
        if (condition(value))
            throw new InvalidOperationException(message);
        return value;
    }
}