using System.Collections.Generic;
using DocumentationWarning.Config;

namespace DocumentationWarning;

public static class Program {
    public static List<ProjectConfig> Configs { get; } = [];

    public static void Main(string[] args) => new Cli().Execute(args).GetAwaiter().GetResult();
}