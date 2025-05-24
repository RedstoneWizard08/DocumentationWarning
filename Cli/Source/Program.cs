namespace DocumentationWarning;

public static class Program {
    public static void Main(string[] args) => new Cli().Execute(args).GetAwaiter().GetResult();
}