namespace CalixManager.Models.Postgrest;

public class Shelf
{
    public string? dbidentity { get; init; }
    public string? sid { get; init; }
    public int referenceid { get; init; }
    public int? autoconnect { get; init; }
    public string? ipaddress1 { get; init; }
    public string? ipaddress2 { get; init; }
    public string? displayname { get; init; }
    public int? port { get; init; }
    public ConnectionState? connectionstate { get; init; }
    //string? synchronize;
    //int synchronizetime;
    //string? networkloginname;
    //string? region;
    //major?
    //minor?
    //point?
}
