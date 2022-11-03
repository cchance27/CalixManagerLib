namespace Calix;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Xml.Linq;
using static CalixXML;

public class CalixManager
{
    
    MessageData messageData;
    CalixManagerConfig configuration;
    Uri netconfApi;
    ILogger<CalixManager> logger;

    public CalixManager(IOptions<CalixManagerConfig> configuration, ILogger<CalixManager> logger)
    {
        this.configuration = configuration.Value;
        this.logger = logger;

        messageData = new MessageData(configuration.Value.username, configuration.Value.password);
        netconfApi = new Uri(configuration.Value.netconfApi);
    }

    public async Task<bool> LoginAsync(HttpClient httpClient)
    {
        var xdoc = await PostAsync(CalixXML.Login(messageData), httpClient);

        var authreply = xdoc.Descendants().Where(v => v.Name.LocalName == "auth-reply").FirstOrDefault();
        if (authreply is not null && authreply.HasElements && authreply?.Element("ResultCode")?.Value == "0")
        {
            messageData.sessionId = Int32.Parse(authreply?.Element("SessionId")?.Value ?? "-1");
            logger.LogInformation($"Calix Session {messageData.sessionId} Logged In.");
            return true;
        }
        else
        {
            throw new Exception("Login to Calix CMS Failed");
        }
    }

    public async Task LogoutAsync(HttpClient httpClient)
    {
        await PostAsync(CalixXML.Logout(messageData), httpClient);
        
        logger.LogInformation($"Calix Session {messageData.sessionId} Logged Out.");
    }

    public async Task<Ont> GetOntAsync(string node, string serial, HttpClient httpClient)
    {
        node = node.ToUpper();

        if (!node.StartsWith("NTWK-"))
            node = "NTWK-" + node;

        var xdoc = await PostAsync(CalixXML.GetOnt(messageData, node, serial), httpClient);

        var config = xdoc.Descendants().Where(v => v.Name.LocalName == "get-config").FirstOrDefault()?.Element("object");
        
        Ont ont = new Ont();
        if (config is not null && config.HasElements)
        {

            ont.id = Int32.Parse(config?.Element("id")?.Element("ont")?.Value ?? "-1");
            ont.ontprof = Int32.Parse(config?.Element("ontprof")?.Element("id")?.Element("ontprof")?.Value ?? "-1");
            ont.enabled = config?.Element("admin")?.Value == "enabled";
            ont.serno = config?.Element("serno")?.Value ?? "";
            ont.subscriberid = config?.Element("subscr-id")?.Value ?? "";
            ont.descr = config?.Element("descr")?.Value ?? "";
            ont.pon = new PonPort
            {
                shelf = Byte.Parse(config?.Element("linked-pon")?.Element("id")?.Element("shelf")?.Value ?? "0"),
                card = Byte.Parse(config?.Element("linked-pon")?.Element("id")?.Element("card")?.Value ?? "0"),
                gponport = Byte.Parse(config?.Element("linked-pon")?.Element("id")?.Element("gponport")?.Value ?? "0"), 
            };

        }

        var devstate = xdoc.Descendants().Where(v => v.Name.LocalName == "get").FirstOrDefault()?.Element("object");

        if (devstate is not null && devstate.HasElements)
        {
            ont.state = new OntState {
                opState = devstate?.Element("op-stat")?.Value ?? "unknown",
                model = devstate?.Element("model")?.Value ?? "unknown",
                vendor = devstate?.Element("vendor")?.Value ?? "CXNK",
                states = devstate?.Element("derived-states")?.Value.Split(" ") ?? Array.Empty<string>(),
                mfgSerNo = devstate?.Element("mfg-serno")?.Value ?? "unknown",
                uptime = Double.Parse(devstate?.Element("uptime")?.Value ?? "-1"),
                optsiglvl = float.Parse(devstate?.Element("opt-sig-lvl")?.Value ?? "-99"),
                feoptlvl = float.Parse(devstate?.Element("fe-opt-lvl")?.Value ?? "-99"),
                range = Int32.Parse(devstate?.Element("range-length")?.Value ?? "-1"),
                dsSdBerRate = Byte.Parse(devstate?.Element("cur-ds-sdber-rate")?.Value ?? "0"),
                usSdBerRate = Byte.Parse(devstate?.Element("cur-us-sdber-rate")?.Value ?? "0"),
                currSw = devstate?.Element("curr-sw-vers")?.Value ?? "unknown",
                altSw = devstate?.Element("alt-sw-vers")?.Value ?? "unknown",
                currCommitted = devstate?.Element("curr-committed")?.Value == "true",
                responsetime = Int32.Parse(devstate?.Element("response-time")?.Value ?? "-1"),
                onuMac = devstate?.Element("onu-mac")?.Value ?? "unknown",
                mtaMac = devstate?.Element("mta-mac")?.Value ?? "unknown"
            };
        }

        return ont;
    }

    async Task<XDocument> PostAsync(string xml, HttpClient httpClient)
    {
        HttpContent content = new StringContent(xml);
        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/json;charset=UTF-8");
        
        var result = await httpClient.PostAsync(netconfApi, content);
        result.EnsureSuccessStatusCode();

        var stream = await result.Content.ReadAsStreamAsync();

        return XDocument.Load(stream);
    }

}
