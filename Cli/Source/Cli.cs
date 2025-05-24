using System.Threading.Tasks;
using CommandLine;
using DocumentationWarning.Commands;

namespace DocumentationWarning;

public sealed class Cli : Command<object> {
    public async Task Execute(string[] args) {
        for (var i = 0; i < args.Length; i++) {
            var arg = args[i];

            if (arg == "-h") {
                args[i] = "--help";
            }
        }

        await Parser.Default
            .ParseArguments<Run.Options, Init.Options, Build.Options, Generate.Options>(args)
            .WithParsedAsync(Run);
    }

    protected override async Task Execute(object obj) {
        switch (obj) {
            case Run.Options r:
                await new Run().Run(r);
                break;

            case Init.Options i:
                await new Init().Run(i);
                break;

            case Build.Options b:
                await new Build().Run(b);
                break;

            case Generate.Options g:
                await new Generate().Run(g);
                break;
        }
    }
}