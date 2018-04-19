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
        public int PlayerOneId { get; set; }
        [DisplayName("Player Two")]
        public int PlayerTwoId { get; set; }
    }
}
