export default {
    defaultTheme: "dark",
    iconLinks: [
        <if [GameHasWebsite]>
        {
            icon: "globe",
            href: "@{{GameWebsite}}",
            title: "Website",
        },
        </if>
        {
            icon: "steam",
            href: "@{{SteamPage}}",
            title: "Steam Page",
        },
        {
            icon: "github",
            href: "https://github.com/RedstoneWizard08/DocumentationWarning",
            title: "GitHub",
        },
        <if [GameHasThunderstore]>
        {
            icon: "download",
            href: "@{{ThunderstoreUrl}}",
            title: "Thunderstore",
        },
        </if>
    ],
};
