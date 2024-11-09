namespace SmallC;

public class CompilationException : Exception
{
    /// <summary>
    /// Report an error
    /// </summary>
    private CompilationException(string s) : base($"\a Error: {s}.")
    {
    }

    /// <summary>
    /// Report error and terminate
    /// </summary>
    public static CompilationException Abort(string s) => new CompilationException(s);

    /// <summary>
    /// Report what was expected
    /// </summary>
    public static CompilationException Expected(string s) => Abort($"{s} Expected");
}
