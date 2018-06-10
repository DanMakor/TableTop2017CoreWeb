using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class RoundMatchup
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
        public int PlayerOneSportsmanshipScore { get; set; }
        [DisplayName("Sportsmanship Grade")]
        public int PlayerTwoSportsmanshipScore { get; set; }
        public int Table { get; set; }

        public RoundMatchupEditViewModel ToRoundMatchupEditViewModel()
        {
            var arevm = new RoundMatchupEditViewModel()
            {
                Id = this.Id,
                RoundNo = this.RoundNo,
                PlayerOneName = this.PlayerOne.Name,
                PlayerOneId = this.PlayerOne.Id,
                PlayerOneBattleScore = this.PlayerOneBattleScore,
                PlayerOneSportsmanshipScore = this.PlayerOneSportsmanshipScore,
                Table = this.Table,
            };
            if (this.PlayerTwo != null)
            {
                arevm.PlayerTwoName = this.PlayerTwo.Name;
                arevm.PlayerTwoId = this.PlayerTwo.Id;
                arevm.PlayerTwoBattleScore = this.PlayerTwoBattleScore;
                arevm.PlayerTwoSportsmanshipScore = this.PlayerTwoSportsmanshipScore;
            }
            else
            {
                arevm.PlayerTwoId = 0.5;
            }
            return arevm;
        }
    }

}
