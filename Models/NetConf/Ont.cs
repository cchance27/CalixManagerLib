namespace CalixManager.Models.NetConf;

public class Ont
{
    public int id { get; set; }
    public int ontprof { get; set; }
    public bool enabled { get; set; }
    public string serno { get; set; } = "";
    public string subscriberid { get; set; } = "";
    public string descr { get; set; } = "";
    public PonLocation? pon { get; set; }
    public OntState? state { get; set; }
}

