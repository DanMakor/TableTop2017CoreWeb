using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableTop2017CoreWeb.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public double BattleScoreRatio { get; set; }
        public double SportsmanshipScoreRatio { get; set; }
        public double ArmyScoreRatio { get; set; }
    }
}
