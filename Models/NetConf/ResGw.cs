using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CalixManager.Models.NetConf.Member;

namespace CalixManager.Models.NetConf
{
    public class ResGw
    {
        public int slot { get; set; }
        public int port { get; set; }
        public bool active { get; set; }
        public string? remoteAccessTime { get; set; }
        public int memberCount { get; set; }
        public Member[]? members {get; set;}
        public int ResGwCount { get; set; }
        public ResGwWan[]? ResGws { get; set; }
    }

    public class Member
    {
        public string? type { get; set; }
        public int port { get; set; }
        public int ontslot { get; set; }
    }

    public class ResGwWan
    {
        public int vlan { get; set; }
        public string? wanProtocol { get; set; } //unconfigured, static, pppoe
        public string? rgStatus { get; set; }
        public string? ip { get; set; }
        public string? ipMask { get; set; }
        public string? ipGw { get; set; }
        public string? mac { get; set; }
    }
}
