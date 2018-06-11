using Microsoft.EntityFrameworkCore;
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
            var preSortedRoundMatchups = _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => !(r is PairRoundMatchup) && r.PlayerTwo != null).ToList();
            var preSortedPairRoundMatchups = _context.RoundMatchups.OfType<PairRoundMatchup>().Include(r => r.PlayerOne).Include(r => r.PlayerThree).Where(r => r.PlayerTwo != null).ToList();
            List<RoundMatchup> roundMatchups = preSortedRoundMatchups.Union(preSortedPairRoundMatchups).OrderBy(r => r.RoundNo).ToList();
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

        //Get the Battle and Sportsmanship Scores of each player based on the RoundMatchup and PairRoundMatchup Entries. Return a Pair of Dictionary objects with each player and their BattleScore + SportsmanshipScore
        public static List<Dictionary<Player, int>> GetAllPlayerScores(TournamentDbContext _context)
        {
            Dictionary<Player, int> playerBattleScores = new Dictionary<Player, int>();
            Dictionary<Player, int> playerSportsmanshipScores = new Dictionary<Player, int>();

            List<Player> players = _context.Players.ToList();
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.Where(r => r.PlayerTwo != null).ToList();
            foreach (Player player in players)
            {
                playerBattleScores.Add(player, 0);
                playerSportsmanshipScores.Add(player, 0);
            }
            foreach (RoundMatchup roundMatchup in roundMatchups)
            {
                playerBattleScores[roundMatchup.PlayerOne] += roundMatchup.PlayerOneBattleScore;
                playerBattleScores[roundMatchup.PlayerTwo] += roundMatchup.PlayerTwoBattleScore;
                playerSportsmanshipScores[roundMatchup.PlayerOne] += roundMatchup.PlayerOneSportsmanshipScore;
                playerSportsmanshipScores[roundMatchup.PlayerTwo] += roundMatchup.PlayerTwoSportsmanshipScore;

                if (roundMatchup is PairRoundMatchup)
                {
                    var pairRoundMatchup = roundMatchup as PairRoundMatchup;
                    playerBattleScores[pairRoundMatchup.PlayerThree] += pairRoundMatchup.PlayerThreeBattleScore;
                    playerBattleScores[pairRoundMatchup.PlayerFour] += pairRoundMatchup.PlayerFourBattleScore;
                    playerSportsmanshipScores[pairRoundMatchup.PlayerThree] += pairRoundMatchup.PlayerThreeSportsmanshipScore;
                    playerSportsmanshipScores[pairRoundMatchup.PlayerFour] += pairRoundMatchup.PlayerFourSportsmanshipScore;
                }
            }
            List<Dictionary<Player, int>> playerScores = new List<Dictionary<Player, int>>()
            {
                playerBattleScores,
                playerSportsmanshipScores
            };
            return playerScores;
        }

        public static int[] GetPlayerScores(int id,TournamentDbContext _context)
        {
            int playerBattleScore = 0;
            int playerSportsmanshipScore = 0;

            var roundMatchups = _context.RoundMatchups
                .Include(r => r.PlayerOne).Include(r => r.PlayerTwo)
                .Where(r => !(r is PairRoundMatchup) && r.PlayerTwo != null && r.PlayerOne.Id == id || r.PlayerTwo.Id == id)
                .ToList();
            var pairRoundMatchups = _context.PairRoundMatchups
                .Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour)
                .Where(r => r.PlayerTwo != null && r.PlayerOne.Id == id || r.PlayerTwo.Id == id || r.PlayerThree.Id == id || r.PlayerFour.Id == id)
                .ToList();
            roundMatchups = roundMatchups.Union(pairRoundMatchups).ToList();

            foreach (RoundMatchup roundMatchup in roundMatchups)
            {
                if (roundMatchup.PlayerOne.Id == id) { playerBattleScore += roundMatchup.PlayerOneBattleScore; }
                if (roundMatchup.PlayerTwo.Id == id) { playerBattleScore += roundMatchup.PlayerTwoBattleScore; }
                if (roundMatchup.PlayerOne.Id == id) { playerSportsmanshipScore += roundMatchup.PlayerOneSportsmanshipScore; }
                if (roundMatchup.PlayerTwo.Id == id) { playerSportsmanshipScore += roundMatchup.PlayerTwoSportsmanshipScore; }
                if (roundMatchup is PairRoundMatchup)
                {
                    var pairRoundMatchup = roundMatchup as PairRoundMatchup;
                    if (pairRoundMatchup.PlayerThree.Id == id) { playerBattleScore += pairRoundMatchup.PlayerThreeBattleScore; }
                    if (pairRoundMatchup.PlayerFour.Id == id) { playerBattleScore += pairRoundMatchup.PlayerFourBattleScore; }
                    if (pairRoundMatchup.PlayerThree.Id == id) { playerSportsmanshipScore += pairRoundMatchup.PlayerThreeSportsmanshipScore; }
                    if (pairRoundMatchup.PlayerFour.Id == id) { playerSportsmanshipScore += pairRoundMatchup.PlayerFourSportsmanshipScore; }
                }
            }
            int[] playerScores = new int[2] {
                playerBattleScore,
                playerSportsmanshipScore
            };
            return playerScores;
        }

        public static void SetPlayerScores(int id, TournamentDbContext _context)
        {
            Player player = _context.Players.SingleOrDefault(p => p.Id == id);

            int[] playerScores = PlayerActions.GetPlayerScores(id, _context);
            int playerBattleScore = playerScores[0];
            int playerSportsmanshipScore = playerScores[1];

            player.BattleScore = playerBattleScore;
            player.SportsmanshipScore = playerSportsmanshipScore;
            _context.SaveChanges();
        }
        //Add SetPlayerScore


        //Validation Function
        //Returns a Dictionary that contains 
        public static Dictionary<Player, List<Player>> GetPreviouslyPlayedOpponentClashes(TournamentDbContext _context)
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
