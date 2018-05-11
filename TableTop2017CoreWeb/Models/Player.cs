using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class Player {
        public int Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        public string Army { get; set; }
        public Boolean Active { get; set; } 
        public Boolean Bye { get; set; }
        [DisplayName("Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        public string Notes { get; set; }
        public Boolean Paid { get; set; }
        [DisplayName("Battle Score")]
        public int BattleScore { get; set; } 
        [DisplayName("Sports Score")]
        public int SportsmanshipScore { get; set; }
        [DisplayName("Army Score")]
        public int ArmyScore { get; set; }
        [DisplayName("Weighted Score")]
        public int WeightedScore { get; set; }
        public Player CurrentOpponent { get; set; }

        public Player()
        {
            Active = true;
            Bye = false;
            BattleScore = 0;
            SportsmanshipScore = 0;
        }
    }
}
