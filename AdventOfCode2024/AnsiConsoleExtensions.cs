namespace AdventOfCode2024;

/// <summary>
/// Wrappers around AnsiConsole that only print when <see cref="DayBase.DebugOutput"/> is true.
/// </summary>
public static class DebugAnsiConsole
{
    /// <inheritdoc cref="AnsiConsole.MarkupLineInterpolated(System.FormattableString)"/>
    public static void MarkupLineInterpolated(FormattableString value)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.MarkupLineInterpolated(value);
    }
    
    /// <inheritdoc cref="AnsiConsole.MarkupLine(string)"/>
    public static void MarkupLine(string text)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.MarkupLine(text);
    }
    
    /// <inheritdoc cref="AnsiConsole.MarkupLine(string, object[])"/>
    public static void MarkupLine(string format, params object[] args)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.MarkupLine(format, args);
    }
    
    /// <inheritdoc cref="AnsiConsole.MarkupLine(IFormatProvider, string, object[])"/>
    public static void MarkupLine(IFormatProvider provider, string format, params object[] args)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.MarkupLine(provider, format, args);
    }
    
    /// <inheritdoc cref="AnsiConsole.MarkupInterpolated(IFormatProvider, FormattableString)"/>
    public static void MarkupInterpolated(IFormatProvider provider, FormattableString value)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.MarkupInterpolated(provider, value);
    }
    
    /// <inheritdoc cref="AnsiConsole.MarkupInterpolated(FormattableString)"/>
    public static void MarkupInterpolated(FormattableString value)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.MarkupInterpolated(value);
    }
    
    /// <inheritdoc cref="AnsiConsole.Markup(string)"/>
    public static void Markup(string value)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.Markup(value);
    }

    /// <inheritdoc cref="AnsiConsole.Markup(string, object[])"/>
    public static void Markup(string format, params object[] args)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.Markup(format, args);
    }
    
    /// <inheritdoc cref="AnsiConsole.Markup(IFormatProvider, string, object[])"/>
    public static void Markup(IFormatProvider provider, string format, params object[] args)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.Markup(provider, format, args);
    }

    /// <inheritdoc cref="AnsiConsole.Write(string)"/>
    public static void Write(string text)
    {
        if (DayBase.DebugOutput)
            AnsiConsole.Write(text);
    }
}