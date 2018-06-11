using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class PairRoundMatchupEditViewModel
    {
        public int Id { get; set; }
        [DisplayName("Round Number")]
        public int RoundNo { get; set; }
        public string PlayerOneName { get; set; }
        public string PlayerTwoName { get; set; }
        public string PlayerThreeName { get; set; }
        public string PlayerFourName { get; set; }
        [DisplayName("Player One")]
        public double PlayerOneId { get; set; }
        [DisplayName("Player Two")]
        public double PlayerTwoId { get; set; }
        [DisplayName("Player Three")]
        public double PlayerThreeId { get; set; }
        [DisplayName("Player Four")]
        public double PlayerFourId { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerOneBattleScore { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerTwoBattleScore { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerThreeBattleScore { get; set; }
        [DisplayName("BattleScore")]
        public int PlayerFourBattleScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public int PlayerOneSportsmanshipScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public int PlayerTwoSportsmanshipScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public int PlayerThreeSportsmanshipScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public int PlayerFourSportsmanshipScore { get; set; }
        public int Table { get; set; }
        public List<Player> Players { get; set; }
    }
}
