using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DocumentationWarning.Config;
using DocumentationWarning.Util;
using SixLabors.ImageSharp;

namespace DocumentationWarning.Processors;

public sealed partial class TemplateProcessor : WithLogger
{
    public required string Site;
    public required string Game;
    public required string Publisher;
    public required string SteamPage;
    public required string Namespace;
    public required string Framework;
    public required string OutDir;
    public required string BannerPath;
    public required string IconPath;
    public required string UnityVersion;
    public required (int, int) BannerSize;
    public string? GameWebsite;
    public string? ThunderstoreUrl;

    public static TemplateProcessor FromConfig(RootConfig root, ProjectConfig config)
    {
        var bannerSize = (1, 1);

        using (var img = Image.Load(config.Game.BannerPath(config)))
        {
            bannerSize = (img.Width, img.Height);
        }

        return new TemplateProcessor()
        {
            Site = root.Site,
            Game = config.Game.Name,
            Publisher = config.Game.Publisher,
            SteamPage = config.Urls.Steam,
            Namespace = config.Game.Namespace,
            Framework = config.Framework.Package,
            OutDir = Path.GetFullPath(config.OutDir),
            BannerPath = config.Game.Banner,
            IconPath = config.Game.Icon,
            UnityVersion = config.Game.Unity,
            GameWebsite = config.Urls.Website,
            ThunderstoreUrl = config.Urls.Thunderstore,
            BannerSize = bannerSize,
        };
    }

    public bool GameHasWebsite
    {
        get => GameWebsite != null;
    }

    public bool GameHasThunderstore
    {
        get => ThunderstoreUrl != null;
    }

    public int ImageWidth
    {
        get => GetScaledWidth(BannerSize);
    }

    public int ImageWidthSmall
    {
        get => GetScaledWidthSmall(BannerSize);
    }

    public const int ScaledImageHeight = 64;
    public const int ScaledImageHeightSmall = 48;

    public Dictionary<string, string> Replacements
    {
        get
        {
            var dict = new Dictionary<string, string>
            {
                { "Site", Site },
                { "Game", Game },
                { "Publisher", Publisher },
                { "SteamPage", SteamPage },
                { "Namespace", Namespace },
                { "Framework", Framework },
                { "OutDir", OutDir },
                { "BannerPath", BannerPath },
                { "IconPath", IconPath },
                { "UnityVersion", UnityVersion },
                { "GameWebsite", GameWebsite ?? "null" },
                { "ThunderstoreUrl", ThunderstoreUrl ?? "null" },
                { "ImageWidth", ImageWidth.ToString() },
                { "ImageWidthSmall", ImageWidthSmall.ToString() },
                { "ScaledImageHeight", ScaledImageHeight.ToString() },
                { "ScaledImageHeightSmall", ScaledImageHeightSmall.ToString() }
            };

            return dict;
        }
    }

    public static int GetScaledWidth((int, int) size)
    {
        var (width, height) = size;

        return width / (height / ScaledImageHeight);
    }

    public static int GetScaledWidthSmall((int, int) size)
    {
        var (width, height) = size;

        return width / (height / ScaledImageHeightSmall);
    }

    public string ProcessConditions(string content)
    {
        var match = IfRegex().Match(content);

        while (match.Success)
        {
            var data = match.Groups[0].Value;
            var cond = match.Groups[1].Value;
            var value = match.Groups[2].Value;

            if (cond == "GameHasWebsite" && GameHasWebsite)
            {
                content = content.Replace(data, value);
            }
            else if (cond == "GameHasThunderstore" && GameHasThunderstore)
            {
                content = content.Replace(data, value);
            }
            else
            {
                content = content.Replace(data, "");
            }

            match = IfRegex().Match(content);
        }

        return content;
    }

    public string Process(string template)
    {
        template = ProcessConditions(template);

        foreach (var (key, val) in Replacements)
        {
            template = template.Replace($"@{{{{{key}}}}}", val);
        }

        return template;
    }

    [GeneratedRegex(@"<if \[([^\]]+)\]>([^<]+)<\/if>", RegexOptions.Multiline)]
    private static partial Regex IfRegex();
}
