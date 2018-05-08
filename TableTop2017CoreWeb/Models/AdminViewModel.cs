using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class AdminViewModel
    {
        public int[] NoOfRounds { get; set; }
        public IEnumerable<RoundMatchup> RoundMatchup { get; set; }
        [DisplayName("Select Round")]
        public string CurrentRound { get; set; }
    }
}
