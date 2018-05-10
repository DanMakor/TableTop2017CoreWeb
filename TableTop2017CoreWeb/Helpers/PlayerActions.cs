using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTop2017CoreWeb.Data;
using TableTop2017CoreWeb.Models;

namespace TableTop2017CoreWeb.Helpers
{
    public class PlayerActions
    {
        public static List<Player> GetAllOpponents(Player player, TournamentDbContext _context)
        {
            List<Player> opponents = new List<Player>();
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.ToList();

            //Loop through all round matchups and add the players opponents for each round to the list
            foreach (var roundMatchup in roundMatchups)
            {
                //Must add both opponents if this round is a pair round
                if (roundMatchup is PairRoundMatchup)
                {
                    var pairRoundMatchup = roundMatchup as PairRoundMatchup;
                    if (pairRoundMatchup.PlayerOne == player)
                    {
                        opponents.Add(pairRoundMatchup.PlayerThree);
                        opponents.Add(pairRoundMatchup.PlayerFour);
                    }
                    else if (pairRoundMatchup.PlayerTwo == player)
                    {
                        opponents.Add(pairRoundMatchup.PlayerThree);
                        opponents.Add(pairRoundMatchup.PlayerFour);
                    }
                    else if (pairRoundMatchup.PlayerThree == player)
                    {
                        opponents.Add(pairRoundMatchup.PlayerOne);
                        opponents.Add(pairRoundMatchup.PlayerTwo);
                    }
                    else if (pairRoundMatchup.PlayerFour == player)
                    {
                        opponents.Add(pairRoundMatchup.PlayerOne);
                        opponents.Add(pairRoundMatchup.PlayerTwo);
                    }
                }
                //Adding opponents for a standard round
                else
                {
                    if (roundMatchup.PlayerOne == player)
                    {
                        opponents.Add(roundMatchup.PlayerTwo);
                    }
                    else if (roundMatchup.PlayerTwo == player)
                    {
                        opponents.Add(roundMatchup.PlayerOne);
                    }
                }
            }
            return opponents;
        }

        //Get the BattleScores of each player based on the RoundMatchup and PairRoundMatchup Entries. Return a Dictionary object with each player and their BattleScore
        public static Dictionary<Player, int> GetAllPlayerBattleScores(TournamentDbContext _context)
        {
            Dictionary<Player, int> playerBattleScores = new Dictionary<Player, int>();
            List<Player> players = _context.Players.ToList();
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.ToList();
            foreach (Player player in players)
            {
                playerBattleScores.Add(player, 0);
            }
            foreach (RoundMatchup roundMatchup in roundMatchups)
            {
                playerBattleScores[roundMatchup.PlayerOne] += roundMatchup.PlayerOneBattleScore;
                playerBattleScores[roundMatchup.PlayerTwo] += roundMatchup.PlayerTwoBattleScore;
            }
            return playerBattleScores;
        }

        //Validation Function
        //Returns a Dictionary that contains 
        public Dictionary<Player, List<Player>> GetPreviouslyPlayedOpponentClashes(TournamentDbContext _context)
        {
            List<Player> players = _context.Players.ToList();
            Dictionary<Player, List<Player>> duplicateOpponents = new Dictionary<Player, List<Player>>();
            foreach (Player player in players)
            {
                List<Player> opponents = GetAllOpponents(player, _context);
                var duplicates = opponents
                    .GroupBy(i => i)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                duplicateOpponents[player] = duplicates;
            }

            return duplicateOpponents;
        }
    }
}
