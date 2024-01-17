using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalixManager.Models.NetConf
{
    public class GponPortState
    {
        public int shelf { get; set; }
        public int card { get; set; }
        public int port { get; set; }
        public bool opStat { get; set; }
        public string? status { get; set; }
        public string? sfpStatus { get; set; }
        public string? sfpConn { get; set; }
        public float sfpTemp { get; set; }
        public float sfpLineLength { get; set; }
        public float sfpWavelength { get; set; }
        public float sfpTxPower { get; set; }
        public float sfpRxPower { get; set; }
        public float sfpTxBias { get; set; }
        public float sfpVoltage { get; set; }
    }
}
