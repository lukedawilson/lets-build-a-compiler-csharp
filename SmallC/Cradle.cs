namespace SmallC;

public class Cradle
{
    public Cradle(string input)
    {
        this.input = input;
        GetChar();
    }

    private readonly string input;
    private int i = 0;

    public char Look { get; private set; }

    /// <summary>
    /// Read new character ftom input stream
    /// </summary>
    public void GetChar()
    {
        Look = i < input.Length ? input[i++] : default;
    }

    /// <summary>
    /// Match a specific input character
    /// </summary>
    public void Match(char x)
    {
        if (Look == x)
        {
            GetChar();
        }
        else
        {
            throw CompilationException.Expected($"'{x}'");
        }
    }

    /// <summary>
    /// Recognise an alpha character
    /// </summary>
    public static bool IsAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

    /// <summary>
    /// Recognise a decimal digit
    /// </summary>
    public static bool IsDigit(char c) => c >= '0' && c <= '9';

    /// <summary>
    /// Get an identifier
    /// </summary>
    public char GetName()
    {
        if (!IsAlpha(Look)) throw CompilationException.Expected("Name");

        var getName = Look.ToString().ToUpper()[0];
        GetChar();
        return getName;
    }

    /// <summary>
    /// Get a number
    /// </summary>
    public char GetNum()
    {
        if (!IsDigit(Look)) throw CompilationException.Expected("Integer");

        var getNum = Look.ToString().ToUpper()[0];
        GetChar();
        return getNum;
    }
}
