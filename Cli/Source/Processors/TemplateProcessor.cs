using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentationWarning.Config;
using DocumentationWarning.Models;
using DocumentationWarning.Util;
using SixLabors.ImageSharp;

namespace DocumentationWarning.Processors;

public sealed partial class TemplateProcessor : WithLogger {
    public required string Site;
    public required string Game;
    public required string Publisher;
    public required string SteamPage;
    public required string Namespace;
    public required string Framework;
    public required string OutDir;
    public required string AsmDir;
    public required string BannerPath;
    public required string IconPath;
    public required string UnityVersion;
    public required (int, int) BannerSize;
    public required List<string> Assemblies;
    public string? GameWebsite;
    public string? ThunderstoreUrl;
    public required bool IsStripped;

    public static TemplateProcessor FromConfig(RootConfig root, ProjectConfig config) {
        (int, int) bannerSize;

        using (var img = Image.Load(config.Game.BannerPath(config))) {
            bannerSize = (img.Width, img.Height);
        }

        return new TemplateProcessor {
            Site = root.Site,
            Game = config.Game.Name,
            Publisher = config.Game.Publisher,
            SteamPage = config.Urls.Steam,
            Namespace = config.Game.Namespace,
            Framework = config.Framework,
            OutDir = Path.GetFullPath(config.OutDir),
            AsmDir = Path.GetFullPath(config.AsmDir),
            Assemblies = config.Assemblies,
            BannerPath = config.Game.Banner,
            IconPath = config.Game.Icon,
            UnityVersion = config.Game.Unity,
            GameWebsite = config.Urls.Website,
            ThunderstoreUrl = config.Urls.Thunderstore,
            BannerSize = bannerSize,
            IsStripped = config.Game.Source != DepSource.Steam,
        };
    }

    public bool GameHasWebsite {
        get => GameWebsite != null;
    }

    public bool GameHasThunderstore {
        get => ThunderstoreUrl != null;
    }

    public int ImageWidth {
        get => GetScaledWidth(BannerSize);
    }

    public int ImageWidthSmall {
        get => GetScaledWidthSmall(BannerSize);
    }

    public const int ScaledImageHeight = 64;
    public const int ScaledImageHeightSmall = 48;

    public Dictionary<string, string> Replacements {
        get {
            var dict = new Dictionary<string, string> {
                { "Site", Site },
                { "Game", Game },
                { "Publisher", Publisher },
                { "SteamPage", SteamPage },
                { "Namespace", Namespace },
                { "Framework", Framework },
                { "OutDir", OutDir },
                { "AsmDir", AsmDir },
                { "BannerPath", BannerPath },
                { "IconPath", IconPath },
                { "UnityVersion", UnityVersion },
                { "GameWebsite", GameWebsite ?? "null" },
                { "ThunderstoreUrl", ThunderstoreUrl ?? "null" },
                { "ImageWidth", ImageWidth.ToString() },
                { "ImageWidthSmall", ImageWidthSmall.ToString() },
                { "ScaledImageHeight", ScaledImageHeight.ToString() },
                { "ScaledImageHeightSmall", ScaledImageHeightSmall.ToString() },
                { "Assemblies", string.Join(',', Assemblies.Select(it => $"\"{it}\"")) }
            };

            return dict;
        }
    }

    private static int GetScaledWidth((int, int) size) {
        var (width, height) = size;

        return width / (height / ScaledImageHeight);
    }

    private static int GetScaledWidthSmall((int, int) size) {
        var (width, height) = size;

        return width / (height / ScaledImageHeightSmall);
    }

    private string ProcessConditions(string content) {
        var match = IfRegex().Match(content);

        while (match.Success) {
            var data = match.Groups[0].Value;
            var cond = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            switch (cond) {
                case "GameHasWebsite" when GameHasWebsite:
                case "GameHasThunderstore" when GameHasThunderstore:
                case "IsStripped" when IsStripped:
                case "IsNotStripped" when !IsStripped:
                    content = content.Replace(data, value);
                    break;
                default:
                    content = content.Replace(data, "");
                    break;
            }

            match = IfRegex().Match(content);
        }

        return content;
    }

    public string Process(string template) {
        template = ProcessConditions(template);

        foreach (var (key, val) in Replacements) {
            template = template.Replace($"@{{{{{key}}}}}", val.Replace("\\", @"\\"));
        }

        return template;
    }

    [GeneratedRegex(@"<if \[([^\]]+)\]>([^<]+)<\/if>", RegexOptions.Multiline)]
    private static partial Regex IfRegex();
}