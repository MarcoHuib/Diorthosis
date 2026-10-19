if (args.Length == 1 && args[0] is "--version" or "-v")
{
    Console.WriteLine("Diorthosis 0.1.0");
    return 0;
}

Console.WriteLine("Diorthosis is not yet connected to an inference engine. Use --version for version information.");
return args.Length == 0 ? 0 : 2;
