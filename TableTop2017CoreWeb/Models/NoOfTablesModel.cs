using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class NoOfTablesModel
    {
        public int Id { get; set; }
        public int NoOfTables { get; set; }
        public List<Player> Players { get; set; }
    }
}
