using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class Player {
        public int id { get; set; }
        [DisplayName("First Name")]
        public string firstName { get; set; }
        [DisplayName("Last Name")]
        public string lastName { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string emailAddress { get; set; }

        [DisplayName("Notes")]
        public string notes { get; set; }
        [DisplayName("Paid")]
        public Boolean hasPaid { get; set; }
        public int roundBattleScore { get; set; }
        [DisplayName("BattleScore")]
        public int totalBattleScore { get; set; }
        public int sportsmanshipScore { get; set; }
        public Player currentOpponent { get; set; }
    }
}
