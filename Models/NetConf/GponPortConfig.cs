using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalixManager.Models.NetConf
{
    public class GponPortConfig
    {
        public int shelf { get; set; }
        public int card { get; set; }
        public int port { get; set; }
        public bool enabled { get; set; }
        public int rateLimit { get; set; }
        public string? descr { get; set; }
    }
}
