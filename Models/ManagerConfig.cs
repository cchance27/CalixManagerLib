namespace CalixManager.Models;

public class ManagerConfig
{
    public string? netconfApi { get; set; }
    public string? postgrest { get; set; }
    public string username { get; set; } = "rootgod";
    public string password { get; set; } = "root";
}
