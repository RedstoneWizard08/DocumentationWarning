using System.Threading.Tasks;

namespace Extractor.Steps;

public abstract class Step: WithLogger
{
    public abstract Task Run();
}
