namespace CalixManager;

using CalixManager.Models;
using CalixManager.Models.NetConf;
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

    public async Task<Models.NetConf.OntProfile> GetOntProfileAsync(string node, int model, CancellationToken token = default)
    {
        node = node.ToUpper();

        if (!node.StartsWith("NTWK-"))
            node = "NTWK-" + node;

        var xdoc = await PostAsync(XMLMessages.GetOntProfile(messageData, node, model), token);

        var config = xdoc.Descendants().Where(v => v.Name.LocalName == "object").FirstOrDefault();

        if (config is not null && config.HasElements)
        {
            return new OntProfile
            {
                name = config?.Element("name")?.Value,
                vendor = config?.Element("vendor")?.Value,
                pots = Int32.Parse(config?.Element("pots")?.Value ?? "0"),
                feth = Int32.Parse(config?.Element("feth")?.Value ?? "0"),
                geth = Int32.Parse(config?.Element("geth")?.Value ?? "0"),
                hpnaeth = Int32.Parse(config?.Element("hpnaeth")?.Value ?? "0"),
                ds1 = Int32.Parse(config?.Element("ds1")?.Value ?? "0"),
                rfvid = Int32.Parse(config?.Element("rfvid")?.Value ?? "0"),
                hotrfvid = Int32.Parse(config?.Element("hotrfvid")?.Value ?? "0"),
                rg = Int32.Parse(config?.Element("rg")?.Value ?? "0"),
                fb = Int32.Parse(config?.Element("fb")?.Value ?? "0"),
            };
        }

        throw new Exception("Unrecognized ONT Profile Model");
    }


    public async Task<Models.NetConf.EthSvcConfig[]?> GetEthSvcsAsync(string node, int ontId, CancellationToken token = default)
    {
        node = node.ToUpper();

        if (!node.StartsWith("NTWK-"))
            node = "NTWK-" + node;

        var xdoc = await PostAsync(XMLMessages.GetAllOntEthSvcConfig(messageData, node, ontId), token) ;
        var children = xdoc.Descendants().Where(v => v.Name.LocalName == "children").FirstOrDefault();

        if (children is not null && children.HasElements)
        {
            return children.Elements().Select(c => new Models.NetConf.EthSvcConfig
            {
                enabled = c.Element("admin")?.Value == "enabled",
                port = Int32.Parse(c.Element("id")?.Element("ontethany")?.Value ?? "0"),
                slot = Int32.Parse(c.Element("id")?.Element("ontslot")?.Value ?? "0"),
                inTag = c.Element("in-tag")?.Value == "none" ? 0 : Int32.Parse(c.Element("in-tag")?.Value ?? "0"),
                outTag = c.Element("out-tag")?.Value == "none" ? 0 : Int32.Parse(c.Element("out-tag")?.Value ?? "0"),
                ethsvcname = c.Element("id")?.Element("ethsvc")?.Attribute("name")?.Value ?? "Unknown",
                bwProfName = c.Element("bw-prof")?.Element("id")?.Element("bwprof")?.Attribute("name")?.Value ?? "Unknown",
                tagActionName = c.Element("tag-action")?.Element("id")?.Element("svctagaction")?.Attribute("name")?.Value ?? "Unknown",
            }).ToArray();
        }

        return null;
    }

    public async Task<Models.NetConf.ResGwConfig[]?> GetResGwConfigsAsync(string node, int ontId, CancellationToken token = default)
    {
        node = node.ToUpper();

        if (!node.StartsWith("NTWK-"))
            node = "NTWK-" + node;

        var xdoc = await PostAsync(XMLMessages.GetAllOntRgConfig(messageData, node, ontId), token);
        var children = xdoc.Descendants().Where(v => v.Name.LocalName == "children").FirstOrDefault();

        if (children is not null && children.HasElements)
        {
            return children.Elements().Select(c => new Models.NetConf.ResGwConfig
            {
                port = Int32.Parse(c.Element("id")?.Element("ontrg")?.Value ?? "0"),
                slot = Int32.Parse(c.Element("id")?.Element("ontslot")?.Value ?? "0"),
                enabled = c.Element("admin")?.Value == "enabled",
                wanProtocol = c.Element("wan-protocol")?.Value,
                pppoeUser = c.Element("pppoe-user")?.Value,
                //pppoePassword = c.Element("pppoe-password")?.Value, // Commented out not sure we want to expose this.
                staticIp = c.Element("static-ip")?.Value,
                staticIpMask = c.Element("static-ip-mask")?.Value,
                staticIpGw = c.Element("static-ip-gw")?.Value,
                priDnsServer = c.Element("pri-dns-server")?.Value,
                secDnsServer = c.Element("sec-dns-server")?.Value,
                setRemoteAccessSecs = Int32.Parse(c.Element("set-remote-access-secs")?.Value ?? "0"),
            }).ToArray();
        }

        return null;
    }

    public async Task<Models.NetConf.ResGw?> GetResGwStateAsync(string node, int ontId, int resGw, CancellationToken token = default)
    {
        node = node.ToUpper();

        if (!node.StartsWith("NTWK-"))
            node = "NTWK-" + node;

        var xdoc = await PostAsync(XMLMessages.GetResGwState(messageData, node, ontId, resGw), token);

        var config = xdoc.Descendants().Where(v => v.Name.LocalName == "object").FirstOrDefault();
        var resgw = xdoc.Descendants().Where(v => v.Name.LocalName == "rg-wan").FirstOrDefault();
        var mbrs = xdoc.Descendants().Where(v => v.Name.LocalName == "mbrs").FirstOrDefault();

        Member[]? allMembers =  Array.Empty<Member>();
        if (mbrs is not null && mbrs.HasElements)
        {
            var m = mbrs.Elements().Select(x => new Member
            {
                type = x.Element("type")?.Value ?? "Unknown",
                ontslot = Int32.Parse(x.Element("id")?.Element("ontslot")?.Value ?? "-1"),
                port = Int32.Parse(x.Element("id")?.Element("ontethge")?.Value ?? "-1")
            });

            allMembers = m.Where(x => x.type != "unknown").ToArray();
        }

        ResGwWan[]? allRgWans = Array.Empty<ResGwWan>();
        if (resgw is not null && resgw.HasElements)
        {
            var m = resgw.Elements().Select(x => new ResGwWan
            {
                vlan = Int32.Parse(x.Element("vlan")?.Value ?? "65535"),
                rgStatus = x.Element("rg-status")?.Value,
                wanProtocol = x.Element("wan-protocol")?.Value,
                mac = x.Element("mac")?.Value,
                ip = x.Element("ip")?.Value,
                ipGw = x.Element("ip-gw")?.Value,
                ipMask = x.Element("ip-mask")?.Value,
            });

            allRgWans = m.Where(x => x.vlan != 65535).ToArray();
        }

        if (config is not null && config.HasElements)
        {
            return new ResGw
            {
                port = Int32.Parse(config.Element("id")?.Element("ontrg")?.Value ?? "0"),
                slot = Int32.Parse(config.Element("id")?.Element("ontslot")?.Value ?? "0"),
                active = config.Element("op-stat")?.Value == "enable",
                remoteAccessTime = config.Element("remote-access-time")?.Value ?? "disabled",
                memberCount = Int32.Parse(config.Element("mbr-count")?.Value ?? "0"),
                members = allMembers,
                ResGwCount = Int32.Parse(config.Element("rg-wan-count")?.Value ?? "0"),
                ResGws = allRgWans,
            };
        }

        return null;
    }

    public async Task<Models.NetConf.Ont> GetOntAsync(string node, string serial, CancellationToken token = default)
    {
        node = node.ToUpper();

        if (!node.StartsWith("NTWK-"))
            node = "NTWK-" + node;

        Models.NetConf.Ont ont = new();
        
        var xdoc = await PostAsync(XMLMessages.GetOnt(messageData, node, serial), token);

        var config = xdoc.Descendants().Where(v => v.Name.LocalName == "get-config").FirstOrDefault()?.Element("object");
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
            ont.state = new Models.NetConf.OntState
            {
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

        ont.resGateways = await GetAllOntResGws(node, ont.id, ont.ontprof, token);
        ont.ethSvcConfig = await GetEthSvcsAsync(node, ont.id, token);
        ont.resGatewayConfig = await GetResGwConfigsAsync(node, ont.id, token);

        return ont;
    }

    private async Task<ResGw[]> GetAllOntResGws(string node, int id, int ontProfile, CancellationToken token)
    {
        var ontProf = await GetOntProfileAsync(node, ontProfile, token);

        // RGs        
        List<ResGw> RGs = new();
        for (var i = 1; i <= ontProf.rg; i++)
        {
            var r = await GetResGwStateAsync(node, id, i, token);
            if (r is not null)
                RGs.Add(r);
        }
        return RGs.ToArray();
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
    public async Task<Models.Postgrest.GlobalBwProfile[]?> GetAllGlobalBwProfilesAsync(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.GlobalBwProfile[]>(postgrest + "globalexontethbwprofile", token);

    /// <summary>
    /// Get All CMS Network Inventory (OLT/DSLAM etc)
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Array of InventoryDevices (CalixManager.Models.Postgrest.InventoryDevice[])</returns>
    public async Task<Models.Postgrest.Shelf[]?> GetAllShelvesAsync(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.Shelf[]>(postgrest + "baseinventorynetwork", token);

    /// <summary>
    /// Get All PonPorts from all OLTs
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Array of OltPonPorts (CalixManager.Models.Postgrest.PonInterface[]</returns>
    public async Task<Models.Postgrest.PonInterface[]?> GetAllPonInterfacesAsync(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.PonInterface[]>(postgrest + "ex_pon", token);

    /// <summary>
    /// Get All Onts from All OLTs (based on CMS Database last poll, not a live polling of ONT)
    /// </summary>
    /// <param name="token"></param>
    /// <returns>Array of OntStatus (CalixManager.Models.Postgrest.OntState[]</returns>
    public async Task<Models.Postgrest.Ont[]?> GetAllOntsAsync(CancellationToken token = default) =>
        await httpClient.GetFromJsonAsync<Models.Postgrest.Ont[]>(postgrest + "ex_ont", token);
}
