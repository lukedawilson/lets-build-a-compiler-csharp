using SmallC;

var interpreter = new Interpreter(args[0]);

do
{
    switch (interpreter.Look)
    {
        case '?':
            interpreter.Input();
            break;
        case '!':
            interpreter.Output();
            break;
        default:
            interpreter.Assignment();
            break;
    }

    interpreter.NewLine();
} while (interpreter.Look != '.');
