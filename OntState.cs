namespace Calix;

public class OntState
{
    public string opState  { get; set; } = "";
    public string model  { get; set; } = "";
    public string vendor  { get; set; } = "CXNK";
    public string[] states  { get; set; } = Array.Empty<string>();
    public string mfgSerNo  { get; set; } = "";
    public double uptime  { get; set; } = 0;
    public float optsiglvl  { get; set; } = -99f;
    public float feoptlvl  { get; set; } = -99f;
    public int range  { get; set; } = 0;
    public byte dsSdBerRate  { get; set; } = 0;
    public byte usSdBerRate  { get; set; } = 0;
    public string currSw  { get; set; } = "";
    public string altSw  { get; set; } = "";
    public bool currCommitted  { get; set; } = false;
    public double responsetime  { get; set; } = 0;
    public string onuMac  { get; set; } = "";
    public string mtaMac  { get; set; } = "";
}
