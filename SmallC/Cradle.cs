namespace SmallC;

public class ParseException : Exception
{
    /// <summary>
    /// Report an error
    /// </summary>
    private ParseException(string s) : base($"\a Error: {s}.")
    {
    }

    /// <summary>
    /// Report error and terminate
    /// </summary>
    public static ParseException Abort(string s) => new ParseException(s);

    /// <summary>
    /// Report what was expected
    /// </summary>
    public static ParseException Expected(string s) => Abort($"{s} Expected");
}

public class Cradle
{
    public Cradle(string input)
    {
        this.input = input;
        GetChar();
    }

    private const char Tab = '\t';

    private readonly string input;
    private int i = 0;

    private char look;

    /// <summary>
    /// Read new character ftom input stream
    /// </summary>
    private void GetChar()
    {
        look = input[i++];
    }

    /// <summary>
    /// Match a specific input character
    /// </summary>
    private void Match(char x)
    {
        if (look == x)
        {
            GetChar();
        }
        else
        {
            throw ParseException.Expected($"'{x}'");
        }
    }

    /// <summary>
    /// Recognise an alpha character
    /// </summary>
    private static bool IsAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

    /// <summary>
    /// Recognise a decimal digit
    /// </summary>
    private static bool IsDigit(char c) => c >= '0' && c <= '9';

    /// <summary>
    /// Get an identifier
    /// </summary>
    private char GetName()
    {
        if (!IsAlpha(look)) throw ParseException.Expected("Name");

        var getName = look.ToString().ToUpper()[0];
        GetChar();
        return getName;
    }

    /// <summary>
    /// Get a number
    /// </summary>
    private char GetNum()
    {
        if (!IsDigit(look)) throw ParseException.Expected("Integer");

        var getNum = look.ToString().ToUpper()[0];
        GetChar();
        return getNum;
    }

    private static void Emit(string s) => Console.Write($"{Tab}s");
    private static void EmitLn(string s) => Console.WriteLine($"{Tab}s");
}