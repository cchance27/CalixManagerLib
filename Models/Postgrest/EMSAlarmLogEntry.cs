using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalixManager.Models.Postgrest
{
    public class EMSAlarmLogEntry
    {
        public uint u_id { get; init; }
        public string? sid { get; init; }
        public uint nodeshelfid { get; init; }
        public uint logseqnum { get; init; }
        public string? facility { get; init; }
        public string? location { get; init; }
        public uint c7_time { get; init; }
        public uint alarm { get; init; }   
        public string? alarmstr { get; init; }
        public byte severity { get; init; }
        public byte action { get; init; }
        public string? description { get; init; }
        public byte service_affecting { get; init; }
        public uint device { get; init; }
        public uint objectclass { get; init; }
    }
}
