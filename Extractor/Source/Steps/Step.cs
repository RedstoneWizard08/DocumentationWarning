using System.Threading.Tasks;
using Extractor.Config;

namespace Extractor.Steps;

public abstract class Step: WithLogger
{
    public abstract Task Run(ProjectConfig config);
}
