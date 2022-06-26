using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPanelToSql.Models
{
    public class Log
    {
        public long Time { get; set; }
        public int TimeOffset { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
