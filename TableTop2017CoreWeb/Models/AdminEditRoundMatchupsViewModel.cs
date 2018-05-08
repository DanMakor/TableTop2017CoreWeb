using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class AdminEditRoundMatchupsViewModel
    {
        public int Id { get; set; }
        [DisplayName("Player One")]
        public double PlayerOneId { get; set; }
        [DisplayName("Player Two")]
        public double PlayerTwoId { get; set; }
        [DisplayName("Player Three")]
        public double PlayerThreeId { get; set; }
        [DisplayName("Player Four")]
        public double PlayerFourId { get; set; }

        //Used just in case a player record has the default int value of 0 ruining existence checking
        public AdminEditRoundMatchupsViewModel()
        {
            PlayerOneId = 0.5;
            PlayerTwoId = 0.5;
            PlayerThreeId = 0.5;
            PlayerFourId = 0.5;
        }
    }

}
