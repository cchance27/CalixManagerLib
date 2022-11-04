namespace CalixManager;

using CalixManager.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Xml.Linq;

public class Session
{
    Models.NetConf.MessageData messageData;
    Uri netconfApi;
    string postgrest;
    ILogger<CalixManager.Session> logger;
    HttpClient httpClient;

    public Session(HttpClient httpClient, IOptions<ManagerConfig> configuration, ILogger<CalixManager.Session> logger)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(configuration.Value.netconfApi);
        ArgumentNullException.ThrowIfNullOrEmpty(configuration.Value.postgrest);

        this.logger = logger;
        this.httpClient = httpClient;

        messageData = new Models.NetConf.MessageData(configuration.Value.username, configuration.Value.password);
        netconfApi = new Uri(configuration.Value.netconfApi);
        postgrest = configuration.Value.postgrest;
    }

    public async Task<bool> LoginAsync(CancellationToken token = default)
    {
        var xdoc = await PostAsync(XMLMessages.Login(messageData), token);

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

    //TODO: Failure case
    public async Task LogoutAsync(CancellationToken token = default)
    {
        await PostAsync(XMLMessages.Logout(messageData), token);
        
        logger.LogInformation($"Calix Session {messageData.sessionId} Logged Out.");
    }

    // TODO: Failure cases
    public async Task<Models.NetConf.Ont> GetOntAsync(string node, string serial, CancellationToken token = default)
    {
        node = node.ToUpper();

        if (!node.StartsWith("NTWK-"))
            node = "NTWK-" + node;

        var xdoc = await PostAsync(XMLMessages.GetOnt(messageData, node, serial), token);

        var config = xdoc.Descendants().Where(v => v.Name.LocalName == "get-config").FirstOrDefault()?.Element("object");
        
        Models.NetConf.Ont ont = new();
        if (config is not null && config.HasElements)
        {

            ont.id = Int32.Parse(config?.Element("id")?.Element("ont")?.Value ?? "-1");
            ont.ontprof = Int32.Parse(config?.Element("ontprof")?.Element("id")?.Element("ontprof")?.Value ?? "-1");
            ont.enabled = config?.Element("admin")?.Value == "enabled";
            ont.serno = config?.Element("serno")?.Value ?? "";
            ont.subscriberid = config?.Element("subscr-id")?.Value ?? "";
            ont.descr = config?.Element("descr")?.Value ?? "";
            ont.pon = new Models.NetConf.PonLocation
            {
                shelf = byte.Parse(config?.Element("linked-pon")?.Element("id")?.Element("shelf")?.Value ?? "0"),
                card = byte.Parse(config?.Element("linked-pon")?.Element("id")?.Element("card")?.Value ?? "0"),
                gponport = byte.Parse(config?.Element("linked-pon")?.Element("id")?.Element("gponport")?.Value ?? "0"), 
            };

        }

        var devstate = xdoc.Descendants().Where(v => v.Name.LocalName == "get").FirstOrDefault()?.Element("object");

        if (devstate is not null && devstate.HasElements)
        {
            ont.state = new Models.NetConf.OntState {
                opState = devstate?.Element("op-stat")?.Value ?? "unknown",
                model = devstate?.Element("model")?.Value ?? "unknown",
                vendor = devstate?.Element("vendor")?.Value ?? "CXNK",
                states = devstate?.Element("derived-states")?.Value.Split(" ") ?? Array.Empty<string>(),
                mfgSerNo = devstate?.Element("mfg-serno")?.Value ?? "unknown",
                uptime = double.Parse(devstate?.Element("uptime")?.Value ?? "-1"),
                optsiglvl = float.Parse(devstate?.Element("opt-sig-lvl")?.Value ?? "-99"),
                feoptlvl = float.Parse(devstate?.Element("fe-opt-lvl")?.Value ?? "-99"),
                range = int.Parse(devstate?.Element("range-length")?.Value ?? "-1"),
                dsSdBerRate = byte.Parse(devstate?.Element("cur-ds-sdber-rate")?.Value ?? "0"),
                usSdBerRate = byte.Parse(devstate?.Element("cur-us-sdber-rate")?.Value ?? "0"),
                currSw = devstate?.Element("curr-sw-vers")?.Value ?? "unknown",
                altSw = devstate?.Element("alt-sw-vers")?.Value ?? "unknown",
                currCommitted = devstate?.Element("curr-committed")?.Value == "true",
                responsetime = int.Parse(devstate?.Element("response-time")?.Value ?? "-1"),
                onuMac = devstate?.Element("onu-mac")?.Value ?? "unknown",
                mtaMac = devstate?.Element("mta-mac")?.Value ?? "unknown"
            };
        }

        return ont;
    }

    // TODO: Polly Retries?
    async Task<XDocument> PostAsync(string xml, CancellationToken token = default)
    {
        HttpContent content = new StringContent(xml);
        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/json;charset=UTF-8");
        
        var result = await httpClient.PostAsync(netconfApi, content, token);
        result.EnsureSuccessStatusCode();

        var stream = await result.Content.ReadAsStreamAsync();

        return XDocument.Load(stream);
    }

    /// <summary>
    /// Array of 
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Array of Bandwidth Profiles (CalixManager.Models.Postgrest.GlobalBwProfile[])</returns>
    async Task<Models.Postgrest.GlobalBwProfile[]?> GetGlobalBwProfiles(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.GlobalBwProfile[]>(postgrest + "globalexontethbwprofile", token);

    /// <summary>
    /// Get All CMS Network Inventory (OLT/DSLAM etc)
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Array of InventoryDevices (CalixManager.Models.Postgrest.InventoryDevice[])</returns>
    async Task<Models.Postgrest.InventoryDevice[]?> GetInventory(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.InventoryDevice[]>(postgrest + "baseinventorynetwork", token);

    /// <summary>
    /// Get All PonPorts from all OLTs
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Array of OltPonPorts (CalixManager.Models.Postgrest.PonInterface[]</returns>
    async Task<Models.Postgrest.PonInterface[]?> GetPonInterfaces(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.PonInterface[]>(postgrest + "ex_pon", token);

    /// <summary>
    /// Get All Onts from All OLTs (based on CMS Database last poll, not a live polling of ONT)
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Array of OntStatus (CalixManager.Models.Postgrest.OntState[]</returns>
    async Task<Models.Postgrest.Ont[]?> GetAllOnts(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.Ont[]>(postgrest + "ex_ont", token);
}
