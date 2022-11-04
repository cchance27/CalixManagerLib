namespace CalixManager.Models.Postgrest;
using System.Net;

public class InventoryDevice
{
    public string? dbidentity { get; init; }
    public string? sid { get; init; }
    public int referenceid { get; init; }
    public bool autoconnect { get; init; }
    public IPAddress? ipaddress1 { get; init; }
    public IPAddress? ipaddress2 { get; init; }
    public string? displayname { get; init; }
    public int port { get; init; }
    public ConnectionState? connectionstate { get; init; }
    //string? synchronize;
    //int synchronizetime;
    //string? networkloginname;
    //string? region;
    //major?
    //minor?
    //point?
}
