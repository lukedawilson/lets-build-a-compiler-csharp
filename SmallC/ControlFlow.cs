namespace SmallC;

public class ControlFlow
{
    public ControlFlow(string input)
    {
        this.input = input;
        GetChar();
    }

    private readonly string input;
    private int i;
    private int lCount = 0;

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

    private void Other() => Console.WriteLine(GetName());

    /// <summary>
    /// Generate a Unique Label
    /// </summary>

    private string NewLabel() => $"l{lCount++}";

    /// <summary>
    /// Post a Label To Output
    /// </summary>
    private static void PostLabel(string l) => Console.WriteLine($"{l}:");

    /// <summary>
    /// Recognize and Translate an IF Construct
    /// </summary>
    private void If(string l)
    {
        Match('i');
        Condition();
        var l1 = NewLabel();
        var l2 = l1;
        Console.WriteLine($"BEQ {l1}");
        Block(l);
        if (Look == 'l')
        {
            Match('l');
            l2 = NewLabel();
            Console.WriteLine($"BRA {l2}");
            PostLabel(l1);
            Block(l);
        }
        Match('e');
        PostLabel(l2);
    }

    /// <summary>
    /// Recognize and Translate a Statement Block
    /// </summary>
    private void Block(string l)
    {
        while (!new [] {'e', 'l', 'u'}.Contains(Look))
        {
            switch (Look)
            {
                case 'i': If(l); break;
                case 'w': While(); break;
                case 'p': Loop(); break;
                case 'r': Repeat(); break;
                case 'f': For(); break;
                case 'd': Do(); break;
                case 'b': Break(l); break;
                default: Other(); break;
            }
        }
    }

    /// <summary>
    /// Parse and Translate a WHILE Statement
    /// </summary>
    private void While()
    {
        Match('w');
        var l1 = NewLabel();
        var l2 = NewLabel();
        PostLabel(l1);
        Condition();
        Console.WriteLine($"BEQ {l2}");
        Block(l2);
        Match('e');
        Console.WriteLine($"BRA {l1}");
        PostLabel(l2);
    }

    /// <summary>
    /// Parse and Translate a LOOP Statement
    /// </summary>
    private void Loop()
    {
        Match('p');
        var l1 = NewLabel();
        var l2 = NewLabel();
        PostLabel(l1);
        Block(l2);
        Match('e');
        Console.WriteLine($"BRA {l1}");
        PostLabel(l2);
    }

    /// <summary>
    /// Parse and Translate a REPEAT Statement
    /// </summary>
    private void Repeat()
    {
        Match('r');
        var l1 = NewLabel();
        var l2 = NewLabel();
        PostLabel(l1);
        Block(l2);
        Match('u');
        Condition();
        Console.WriteLine($"BEQ {l1}");
        PostLabel(l2);
    }

    /// <summary>
    /// Parse and Translate a FOR Statement
    /// </summary>
    private void For()
    {
        Match('f');
        var l1 = NewLabel();
        var l2 = NewLabel();
        var name = GetName();
        Match('=');
        Expression();
        Console.WriteLine("SUBQ #1,D0");
        Console.WriteLine($"LEA {name}(PC),A0");
        Console.WriteLine("MOVE D0,(A0)");
        Expression();
        Console.WriteLine("MOVE D0,-(SP)");
        PostLabel(l1);
        Console.WriteLine($"LEA {name}(PC),A0");
        Console.WriteLine("MOVE (A0),D0");
        Console.WriteLine("ADDQ #1,D0");
        Console.WriteLine("MOVE D0,(A0)");
        Console.WriteLine("CMP (SP),D0:");
        Console.WriteLine($"BGT {l2}");
        Block(l2);
        Match('e');
        Console.WriteLine($"BRA {l1}");
        PostLabel(l2);
        Console.WriteLine("ADDQ #2,SP");
    }

    /// <summary>
    /// Parse and Translate a DO Statement
    /// </summary>
    private void Do()
    {
        Match('d');
        var l1 = NewLabel();
        var l2 = NewLabel();
        Expression();
        Console.WriteLine("SUBQ #1,D0");
        PostLabel(l1);
        Console.WriteLine("MOVE D0,-(SP)");
        Block(l2);
        Console.WriteLine("MOVE (SP)+,D0");
        Console.WriteLine($"DBRA D0,{l1}");
        Console.WriteLine("SUBQ #2,SP");
        PostLabel(l2);
        Console.WriteLine("ADDQ #2,SP");
    }

    /// <summary>
    /// Recognize and Translate a BREAK
    /// </summary>
    private void Break(string l)
    {
        Match('b');
        if (l != null)
        {
            Console.WriteLine($"BRA {l}");
        }
        else
        {
            throw CompilationException.Abort("No loop to break from");
        }
    }
    
    /// <summary>
    /// Parse and Translate a Boolean Condition
    /// This version is a dummy
    /// </summary>
    private void Condition()
    {
        Console.WriteLine("<condition>");
    }

    /// <summary>
    /// Parse and Translate an Expression
    /// This version is a dummy
    /// </summary>
    private void Expression()
    {
        Console.WriteLine("<expr>");
    }

    /// <summary>
    /// Parse and Translate a Program
    /// </summary>
    public void Program()
    {
        Block(null);
        if (Look != 'e') throw CompilationException.Expected("End");
        Console.WriteLine("END");
    }
}