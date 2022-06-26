using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPanelToSql.Models
{
    public class UserPanelInfo
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Tipo { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }
}
