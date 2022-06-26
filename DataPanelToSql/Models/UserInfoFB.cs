using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPanelToSql.Models
{
    public class UserInfoFB
    {
        public UserInfoFB()
        {
           Log = new List<Log>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Log> Log { get; set; }
        public object Log_Day { get; set; }

    }
}
