using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class RoundMatchups
    {
        public int id { get; set; }
        public int roundNo { get; set; }
        public Player player { get; set; }
        public Player opponent { get; set; }
        public int battleScore { get; set; }
        public string sportsmanshipPoints { get; set; }
        public int table { get; set; }
    }
}
