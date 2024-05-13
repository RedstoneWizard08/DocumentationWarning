using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Extractor;

public class Http
{
    private static readonly HttpClient client = new();

    public static async Task<HttpContent> Get(string url)
    {
        return (await client.GetAsync(url)).Content;
    }

    public static async Task<NugetManifest> GetManifest(string package)
    {
        var resp = await Get($"https://api.nuget.org/v3-flatcontainer/{package.ToLower()}/index.json");

        return JsonConvert.DeserializeObject<NugetManifest>(await resp.ReadAsStringAsync())!;
    }
}
