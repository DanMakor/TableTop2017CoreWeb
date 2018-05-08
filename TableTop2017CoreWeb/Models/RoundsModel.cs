﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class RoundsModel
    {
        public int Id { get; set; }
        public int RoundNo { get; set; }
        public int NoTableTops { get; set; }
        public List<Player> ActivePlayers { get; set; }
        public List<RoundMatchups> RoundMatchups { get; set; }

    }
}
