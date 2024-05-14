using System.Collections.Generic;
using DocumentationWarning.Config;
using DocumentationWarning.Util;
using Microsoft.Extensions.Logging;

namespace DocumentationWarning;

public class Program : WithLogger
{
    internal static readonly ILoggerFactory Factory = LoggerFactory.Create(b => b.AddConsole());
    public static List<ProjectConfig> Configs { get; private set; } = [];

    public static void Main(string[] args) => new Program().Start(args);

    public void Start(string[] args)
    {
        new Cli().Execute(args).GetAwaiter().GetResult();
    }
}
