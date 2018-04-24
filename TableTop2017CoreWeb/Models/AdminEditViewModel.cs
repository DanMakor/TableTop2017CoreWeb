using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class AdminEditViewModel
    {
        public AdminEditRoundMatchupsViewModel RoundMatchup { get; set; }
        public List<Player> Players { get; set; }
    }
}
