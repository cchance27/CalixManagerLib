namespace CalixManager.Models.Postgrest;

public class GlobalBwProfile
{
    public string? name { get; init; }
    public int networkref { get; init; }
    public int addressid { get; init; }
    public int aidtype { get; init; }
    public string? down_cir { get; init; }
    public string? down_pir { get; init; }
    public string? up_cir { get; init; }
    public string? up_pir { get; init; }
    public byte enabled { get; init; }
    public byte aeontenabled { get; init; }
    public byte syncstate { get; init; }
    public byte isdefault { get; init; }
    public int up_cbs { get; init; }
    public int down_cbs { get; init; }
    public int up_pbs { get; init; }
    public int down_pbs { get; init; }
}
