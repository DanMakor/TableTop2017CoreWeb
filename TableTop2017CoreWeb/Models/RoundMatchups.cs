using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class RoundMatchups
    {
        public int Id { get; set; }
        [DisplayName("Round Number")]
        public int RoundNo { get; set; }
        [DisplayName("Player One")]
        public Player PlayerOne { get; set; }
        [DisplayName("Player Two")]
        public Player PlayerTwo { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerOneBattleScore { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerTwoBattleScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public string PlayerOneSportsmanshipScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public string PlayerTwoSportsmanshipScore { get; set; }
        public int Table { get; set; }
    }
}
