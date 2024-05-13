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

    public static async Task<NuGetManifest> GetManifest(string package, DepSource source)
    {
        var resp = await Get(source.GetManifestUrl(package));

        return JsonConvert.DeserializeObject<NuGetManifest>(await resp.ReadAsStringAsync())!;
    }
}
