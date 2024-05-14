using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Steps;

public abstract class Step : WithLogger
{
    public abstract Task Run(ProjectConfig config);

    public static async Task Run(ProjectConfig config, params Step[] steps)
    {
        foreach (var step in steps)
        {
            await step.Run(config);
        }
    }
}
