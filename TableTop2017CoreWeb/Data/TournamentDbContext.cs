using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTop2017CoreWeb.Models;

namespace TableTop2017CoreWeb.Data
{
    public class TournamentDbContext : DbContext
    {
        public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options) {

        }
        public DbSet<RoundsModel> RoundsModel { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<RoundMatchup> RoundMatchups { get; set; }
        public DbSet<PairRoundMatchup> PairRoundMatchup { get; set; }
    }
}

public static class EntityExtensions
{
    public static void Clear<T>(this DbSet<T> dbSet) where T : class
    {
        dbSet.RemoveRange(dbSet);
    }
}
