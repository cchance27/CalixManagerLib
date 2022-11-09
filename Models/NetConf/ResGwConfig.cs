using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalixManager.Models.NetConf
{
    public class ResGwConfig
    {
        public int slot { get; set; }
        public int port { get; set; }
        public bool enabled { get; set; }
        public string? wanProtocol { get; set; }
        public string? pppoeUser { get; set; }
        public string? pppoePassword { get; set; }
        public string? staticIp { get; set; }
        public string? staticIpMask { get; set; }
        public string? staticIpGw { get; set; }
        public string? priDnsServer { get; set; }
        public string? secDnsServer { get; set; }
        public int setRemoteAccessSecs {get; set;}

    }
}
