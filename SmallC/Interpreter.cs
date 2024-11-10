namespace SmallC;

public class Interpreter
{
    public Interpreter(string input)
    {
        this.input = input;
        GetChar();
    }

    private readonly string input;
    private int i;

    public char Look { get; private set; }
    public int[] Table { get; } = Enumerable.Range(0, 26).Select(_ => 0).ToArray();

    /// <summary>
    /// Read new character ftom input stream
    /// </summary>
    private void GetChar()
    {
        Look = i < input.Length ? input[i++] : default;
    }

    /// <summary>
    /// Match a specific input character
    /// </summary>
    private void Match(char x)
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
    private static bool IsAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

    /// <summary>
    /// Recognise a decimal digit
    /// </summary>
    private static bool IsDigit(char c) => c >= '0' && c <= '9';

    /// <summary>
    /// Regognise an addop
    /// </summary>
    private static bool IsAddop(char c) => new[] { '+', '-' }.Contains(c);

    /// <summary>
    /// Get a Number
    /// </summary>
    private int GetNum()
    {
        var value = 0;
        if (!IsDigit(Look)) throw CompilationException.Expected("Integer");

        while (IsDigit(Look))
        {
            value = 10 * value + Look - '0';
            GetChar();
        }
        
        return value;
    }

    /// <summary>
    /// Get an identifier
    /// </summary>
    private char GetName()
    {
        if (!IsAlpha(Look)) throw CompilationException.Expected("Name");

        var getName = Look.ToString().ToUpper()[0];
        GetChar();
        return getName;
    }

    /// <summary>
    /// Parse and Translate a Math Term
    /// </summary>
    private int Term()
    {
        var value = Factor();
        while (new[] { '*', '/' }.Contains(Look))
        {
            switch (Look)
            {
                case '*':
                    Match('*');
                    value *= Factor();
                    break;
                case '/':
                    Match('/');
                    value /= Factor();
                    break;
            }
        }

        return value;
    }

    /// <summary>
    /// Parse and Translate a Mathematical Factor
    /// </summary>
    private int Factor()
    {
        int factor;

        if (Look == '(')
        {
            Match('(');
            factor = Expression();
            Match(')');
        }
        else if (IsAlpha(Look))
        {
            factor = Table[GetName() - 'A'];
        }
        else
        {
            factor = GetNum();
        }

        return factor;
    }

    /// <summary>
    /// Parse and Translate an Expression
    /// </summary>
    public int Expression()
    {
        int value;
        if (IsAddop(Look))
        {
            value = 0;
        }
        else
        {
            value = Term();
        }

        while (IsAddop(Look))
        {
            switch (Look)
            {
                case '+':
                    Match('+');
                    value += Term();
                    break;
                case '-':
                    Match('-');
                    value -= Term();
                    break;
            }
        }

        return value;
    }

    /// <summary>
    /// Parse and Translate an Assignment Statement
    /// </summary>
    public void Assignment()
    {
        var name = GetName();
        Match('=');
        Table[name - 'A'] = Expression();
    }


    /// <summary>
    /// Recognize and Skip Over a Newline
    /// </summary>
    public void NewLine()
    {
        if (Environment.NewLine == "\r\n")
        {
            if (Look == '\r')
            {
                GetChar();
                if (Look == '\n')
                {
                    GetChar();
                }
            }
        }
        else
        {
            if (Look == '\n')
            {
                GetChar();
            }
        }
    }

    /// <summary>
    /// Input Routine
    /// </summary>
    public void Input()
    {
        Match('?');
        Table[GetName() - 'A'] = Console.ReadKey().KeyChar - '0';
    }

    /// <summary>
    /// Output Routine
    /// </summary>
    public void Output()
    {
        Match('!');
        Console.WriteLine(Table[GetName() - 'A']);
    }
}
