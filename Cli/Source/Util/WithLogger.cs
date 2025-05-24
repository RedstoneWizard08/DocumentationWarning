using DocumentationWarning.Logging;
using Newtonsoft.Json;

namespace DocumentationWarning.Util;

public class WithLogger {
    [JsonIgnore] internal readonly Logger Logger;

    protected WithLogger() {
        Logger = new Logger(GetType());
    }
}