using System.Net;
using System.Net.Http.Headers;

namespace Crawl;

public class Ext
{
    public async Task Init()
    {
        CookieContainer cc = new CookieContainer();
        var socketsHandler = new SocketsHttpHandler();
        socketsHandler.AutomaticDecompression = DecompressionMethods.All;
        socketsHandler.UseCookies = true;
        socketsHandler.CookieContainer = cc;
        socketsHandler.AllowAutoRedirect = true;
        socketsHandler.MaxConnectionsPerServer = Math.Max(1, Environment.ProcessorCount - 1);
        socketsHandler.PooledConnectionLifetime=TimeSpan.FromSeconds(60);
        socketsHandler.PooledConnectionIdleTimeout=TimeSpan.FromMinutes(20);
        socketsHandler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true;
        socketsHandler.UseProxy = false;
        socketsHandler.Proxy = new WebProxy("socks5://127.0.0.1:1080");
        socketsHandler.MaxAutomaticRedirections = 50;
        socketsHandler.ResponseDrainTimeout=TimeSpan.FromSeconds(30);
        var client = new HttpClient(socketsHandler);
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64; Trident/6.0)");
        client.DefaultRequestHeaders.Accept.TryParseAdd(
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip, deflate, br");
        client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("zh-CN");
        client.DefaultRequestHeaders.Pragma.TryParseAdd("no-cache");
        client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue {NoStore = true};
        client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");        
        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1)
        };
        List<string> urls = new List<string>();
        await Parallel.ForEachAsync(urls, parallelOptions, async (uri, token) =>
        {
            var html = await client.GetStringAsync(uri, token);
        });
    }
}