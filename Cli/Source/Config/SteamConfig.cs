using System;
using DocumentationWarning.Util;
using Newtonsoft.Json;

namespace DocumentationWarning.Config;

public class SteamConfig : WithLogger {
    private const string UsernameEnvVar = "STEAM_USERNAME";
    private const string LoginTokenEnvVar = "STEAM_LOGIN_TOKEN";

    public string? Username;
    public string? LoginToken;

    /// <summary>
    /// Get the username from the environment variable, "STEAM_USERNAME".
    /// </summary>
    public bool UsernameFromEnv = false;

    /// <summary>
    /// Get the login token from the environment variable, "STEAM_LOGIN_TOKEN".
    /// </summary>
    public bool LoginTokenFromEnv = false;

    [JsonIgnore]
    public string FinalUsername {
        get {
            if (UsernameFromEnv) {
                var username = Environment.GetEnvironmentVariable(UsernameEnvVar);

                if (!string.IsNullOrEmpty(username)) return username;

                $"Environment variable {UsernameEnvVar} is not set.".LogCritical(this);
                Environment.Exit(1);
                throw null;
            }

            if (!string.IsNullOrEmpty(Username)) return Username;

            "Username is not set.".LogCritical(this);
            Environment.Exit(1);
            throw null;
        }
    }

    [JsonIgnore]
    public string FinalLoginToken {
        get {
            if (LoginTokenFromEnv) {
                var loginToken = Environment.GetEnvironmentVariable(LoginTokenEnvVar);

                if (!string.IsNullOrEmpty(loginToken)) return loginToken;

                $"Environment variable {LoginTokenEnvVar} is not set.".LogCritical(this);
                Environment.Exit(1);
                throw null;
            }

            if (!string.IsNullOrEmpty(LoginToken)) return LoginToken;

            "Login token is not set.".LogCritical(this);
            Environment.Exit(1);
            throw null;
        }
    }
}