using System.Net.Http;
using System.Threading.Tasks;
using DocumentationWarning.Models;
using Newtonsoft.Json;

namespace DocumentationWarning.Util;

public static class Http {
    private static readonly HttpClient client = new();

    public static async Task<HttpContent> Get(string url) => (await client.GetAsync(url)).Content;

    public static async Task<NuGetManifest> GetManifest(string package, DepSource source) =>
        JsonConvert.DeserializeObject<NuGetManifest>(
            await (await Get(source.GetManifestUrl(package))).ReadAsStringAsync()
        )!;
}
