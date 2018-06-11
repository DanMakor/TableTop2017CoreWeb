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

        public PairRoundMatchupEditViewModel ToPairRoundMatchupEditViewModel()
        {
            var prmevm = new PairRoundMatchupEditViewModel()
            {
                Id = this.Id,
                RoundNo = this.RoundNo,
                PlayerOneName = this.PlayerOne.Name,
                PlayerOneId = this.PlayerOne.Id,
                PlayerOneBattleScore = this.PlayerOneBattleScore,
                PlayerOneSportsmanshipScore = this.PlayerOneSportsmanshipScore,
                Table = this.Table,
            };
            //Check for a bye
            if (this.PlayerTwo != null)
            {
                prmevm.PlayerTwoName = PlayerTwo.Name;
                prmevm.PlayerTwoId = PlayerTwo.Id;
                prmevm.PlayerTwoBattleScore = PlayerTwoBattleScore;
                prmevm.PlayerTwoSportsmanshipScore = PlayerTwoSportsmanshipScore;
                prmevm.PlayerThreeName = PlayerThree.Name;
                prmevm.PlayerThreeId = PlayerThree.Id;
                prmevm.PlayerThreeBattleScore = PlayerThreeBattleScore;
                prmevm.PlayerThreeSportsmanshipScore = PlayerThreeSportsmanshipScore;
                prmevm.PlayerFourName = PlayerFour.Name;
                prmevm.PlayerFourId = PlayerFour.Id;
                prmevm.PlayerFourBattleScore = PlayerFourBattleScore;
                prmevm.PlayerFourSportsmanshipScore = PlayerFourSportsmanshipScore;
            }
            else
            {
                prmevm.PlayerTwoId = 0.5;
                prmevm.PlayerThreeId = 0.5;
                prmevm.PlayerFourId = 0.5;
            }
            return prmevm;
        }
    }
}
