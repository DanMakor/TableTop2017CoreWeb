using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTop2017CoreWeb.Models;

namespace TableTop2017CoreWeb.Data
{
    public class DbInitializer
    {
        public static void Initialize(TournamentDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Tournaments.Any())
            {
                return;
            }

            context.Tournaments.Add(new Tournament
            {
                ArmyScoreRatio = 1 / 3,
                BattleScoreRatio = 1 / 3,
                SportsmanshipScoreRatio = 1 / 3
            });

            context.SaveChanges();
        }
    }
}
