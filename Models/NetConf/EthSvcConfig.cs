using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalixManager.Models.NetConf
{
    public class EthSvcConfig
    {
        public int slot { get; set; }
        public int port { get; set; }
        public string? ethsvcname { get; set; }
        public bool enabled { get; set; }
        public int outTag { get; set; }
        public int inTag { get; set; }
        public string? bwProfName { get; set; }
        public string? tagActionName { get; set; }
    }
}
