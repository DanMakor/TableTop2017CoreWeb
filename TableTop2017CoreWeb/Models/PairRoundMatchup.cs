using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class PairRoundMatchup : RoundMatchup
    {
        public Player PlayerThree { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerThreeBattleScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public int PlayerThreeSportsmanshipScore { get; set; }
        public Player PlayerFour { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerFourBattleScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public int PlayerFourSportsmanshipScore { get; set; }
    }
}
