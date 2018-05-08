using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTop2017CoreWeb.Models;

namespace TableTop2017CoreWeb.Helpers
{
    public class PlayerPair
    {
        public Player First { get; set; }
        public Player Second { get; set; }
        public PlayerPair CurrentOpponent { get; set; }
        public PlayerPair()
        {

        }

        public PlayerPair(Player first, Player second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}
