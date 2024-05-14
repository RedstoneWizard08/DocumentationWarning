using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentationWarning.Config;
using DocumentationWarning.Util;

namespace DocumentationWarning.Commands;

public abstract class Command<T> : WithLogger {
    protected List<ProjectConfig> Configs { get; private set; } = [];

    protected ProjectConfig? GetConfig(string? Project) {
        return Configs.Find((v) => v.Game.Id == Project);
    }

    public async Task Run(T options) {
        Configs = await ConfigHelper.GetConfigs();

        await Execute(options);
    }

    public abstract Task Execute(T options);
}
