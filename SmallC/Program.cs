using SmallC;

var compiler = new Parser(args[0]);
compiler.Assignment();

if (compiler.Look != '\n')
{
    throw CompilationException.Expected("Newline");
}
