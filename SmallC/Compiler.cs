namespace SmallC;

public class Compiler(Cradle cradle)
{
    /// <summary>
    /// Regognise and translate a mathematical factor
    /// </summary>
    private void Factor()
    {
        if (cradle.Look == '(')
        {
            cradle.Match('(');
            Expression();
            cradle.Match(')');
        }
        else
        {
            Console.WriteLine($"MOVE #{cradle.GetNum()},D0");
        }
    }

    /// <summary>
    /// Recognise and translate a multiply
    /// </summary>
    private void Multiply()
    {
        cradle.Match('*');
        Factor();
        Console.WriteLine("MULS (SP)+,D0");
    }

    /// <summary>
    /// Recognise and translate a divide
    /// </summary>
    private void Divide()
    {
        cradle.Match('/');
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

        while (new[] {'*', '/'}.Contains(cradle.Look))
        {
            Console.WriteLine("MOVE D0,-(SP)");
            switch (cradle.Look)
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
        cradle.Match('+');
        Term();
        Console.WriteLine("ADD (SP)+,D0");
    }

    /// <summary>
    /// Recognise and translate a subtract
    /// </summary>
    private void Subtract()
    {
        cradle.Match('-');
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
        if (IsAddop(cradle.Look))
        {
            Console.WriteLine("CLR D0");
        }
        else
        {
            Term();
        }

        while (IsAddop(cradle.Look))
        {
            Console.WriteLine("MOVE D0,-(SP)");
            switch (cradle.Look)
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
}