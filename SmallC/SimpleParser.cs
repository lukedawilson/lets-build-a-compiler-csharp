namespace SmallC;

public class SimpleParser
{
    public SimpleParser(string input)
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
    /// Get a number
    /// </summary>
    private char GetNum()
    {
        if (!IsDigit(Look)) throw CompilationException.Expected("Integer");

        var getNum = Look.ToString().ToUpper()[0];
        GetChar();
        return getNum;
    }

    /// <summary>
    /// Regognise and translate a mathematical factor
    /// </summary>
    private void Factor()
    {
        if (Look == '(')
        {
            Match('(');
            Expression();
            Match(')');
        }
        else if (IsAlpha(Look))
        {
            Ident();
        }
        else
        {
            Console.WriteLine($"MOVE #{GetNum()},D0");
        }
    }

    /// <summary>
    /// Parse and Translate an Identifier
    /// </summary>
    private void Ident()
    {
        var name = GetName();
        if (Look == '(')
        {
            Match('(');
            Match(')');
            Console.WriteLine($"BSR {name}");
        }
        else
        {
            Console.WriteLine($"MOVE {name}(PC),D0");
        }
    }

    /// <summary>
    /// Recognise and translate a multiply
    /// </summary>
    private void Multiply()
    {
        Match('*');
        Factor();
        Console.WriteLine("MULS (SP)+,D0");
    }

    /// <summary>
    /// Recognise and translate a divide
    /// </summary>
    private void Divide()
    {
        Match('/');
        Factor();
        Console.WriteLine("MOVE (SP)+,D1");
        Console.WriteLine("DIVS D1,D0");
    }

    /// <summary>
    /// Parse and translate a mathematical term
    /// </summary>
    private void Term()
    {
        Factor();

        while (new[] {'*', '/'}.Contains(Look))
        {
            Console.WriteLine("MOVE D0,-(SP)");
            switch (Look)
            {
                case '*':
                    Multiply();
                    break;
                case '/':
                    Divide();
                    break;
                default:
                    throw CompilationException.Expected("Mulop");
            }
        }
    }

    /// <summary>
    /// Recognise and translate an add
    /// </summary>
    private void Add()
    {
        Match('+');
        Term();
        Console.WriteLine("ADD (SP)+,D0");
    }

    /// <summary>
    /// Recognise and translate a subtract
    /// </summary>
    private void Subtract()
    {
        Match('-');
        Term();
        Console.WriteLine("SUB (SP)+,D0");
        Console.WriteLine("NEG D0");
    }

    /// <summary>
    /// Regognise an addop
    /// </summary>
    private static bool IsAddop(char c) => new[] { '+', '-' }.Contains(c);

    /// <summary>
    /// Parse and translate an expression
    /// </summary>
    public void Expression()
    {
        if (IsAddop(Look))
        {
            Console.WriteLine("CLR D0");
        }
        else
        {
            Term();
        }

        while (IsAddop(Look))
        {
            Console.WriteLine("MOVE D0,-(SP)");
            switch (Look)
            {
                case '+':
                    Add();
                    break;
                case '-':
                    Subtract();
                    break;
                default:
                    throw CompilationException.Expected("Addop");
            }
        }
    }

    /// <summary>
    /// Parse and Translate an Assignment Statement
    /// </summary>
    public void Assignment()
    {
        var name = GetName();
        Match('=');
        Expression();
        Console.WriteLine($"LEA {name}(PC),A0");
        Console.WriteLine("MOVE D0,(A0)");
    }
}