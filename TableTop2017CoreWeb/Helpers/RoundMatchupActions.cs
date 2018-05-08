using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTop2017CoreWeb.Data;
using TableTop2017CoreWeb.Models;

namespace TableTop2017CoreWeb.Helpers
{
    public class RoundMatchupActions
    {
        public static bool RoundMatchupsExists(int id, TournamentDbContext _context)
        {
            return _context.RoundMatchups.Any(e => e.Id == id);
        }

        public static int GetLastRoundNo(TournamentDbContext _context)
        {
            int lastRoundNo = 0;
            RoundMatchup lastRound =  _context.RoundMatchups.LastOrDefault();
            if (lastRound != null)
            {
                lastRoundNo = lastRound.RoundNo;
            }
            return lastRoundNo;
        }

        /**
         * 
         * Round Generation
         * 
         **/
        public static void GenerateNextRound(TournamentDbContext _context)
        {
            List<Player> players = _context.Players.OrderByDescending(p => p.BattleScore).ToList();
            //List<int> AllocatedTables = new List<int>(GetnoOfTables());
            int secondaryIndex = 0;
            int i = 0;
            while (i < players.Count)
            {
                //Skip this player if they are already allocated an opponent
                if (players[i].CurrentOpponent == null)
                {
                    if (secondaryIndex == 0) { secondaryIndex = i + 1; }
                    int s = 0;

                    for (s = secondaryIndex; s < players.Count; s++)
                    {
                        if (players[s].CurrentOpponent == null)
                        {

                            //Check if higher player has ever played lower player
                            var opponents = PlayerActions.GetAllOpponents(players[i], _context);
                            var hasPlayed = false;
                            foreach (Player opponent in opponents)
                            {
                                if (players[s] == opponent) { hasPlayed = true; }
                            }
                            //If they have not played allocate them as opponents
                            if (hasPlayed == false)
                            {
                                players[i].CurrentOpponent = players[s];
                                players[s].CurrentOpponent = players[i];
                                secondaryIndex = 0;
                                break;
                            }

                            /**
                             * Following block is to deallocate the next lowest ranked allocated pair
                             **/
                            if (players.Where(p => p.CurrentOpponent == null).LastOrDefault() == players[s] && (players[i].CurrentOpponent == null))
                            //if (s == (players.Count - 1) && (players[i].CurrentOpponent == null))
                            {
                                if (i - 1 >= 0)
                                {
                                    //Set the lowestAllocatedPair to the highest ranked player
                                    Player lowestAllocatedPair = players[0];
                                    //Iterate from the second highest ranked player all the way to the player ranked one higher than the player currently being matched
                                    for (int playerIndex = 1; playerIndex < i; playerIndex++)
                                    {
                                        //Assign the current player to the player being examined in the current iteration of the loop 
                                        Player currentPlayer = players[playerIndex];

                                        //Check that the current player has an opponent (if not, skip to the next iteration of the loop)
                                        if (currentPlayer.CurrentOpponent != null)
                                        {
                                            //Proceed if the current player's opponent has a higher rank than the current player
                                            if (players.IndexOf(currentPlayer.CurrentOpponent) < players.IndexOf(currentPlayer))
                                            {
                                                if (players.IndexOf(currentPlayer.CurrentOpponent) > players.IndexOf(lowestAllocatedPair))
                                                {
                                                    //Set lowestAllocatedPair to the current player's opponent 
                                                    //(The highest ranked member of the new lowest ranked allocated pair)
                                                    lowestAllocatedPair = currentPlayer.CurrentOpponent;
                                                }
                                            }
                                            //Proceed if the current player has a higher rank than their opponent
                                            else if (players.IndexOf(currentPlayer) < players.IndexOf(currentPlayer.CurrentOpponent))
                                            {
                                                //Proceed if the current player has a lower rank than the previous value of lowestAllocatedPair 
                                                if (players.IndexOf(currentPlayer) > players.IndexOf(lowestAllocatedPair))
                                                {
                                                    //Set lowestAllocatedPair to the current player 
                                                    //(The highest ranked member of the new lowest ranked allocated pair)
                                                    lowestAllocatedPair = currentPlayer;
                                                }
                                            }
                                        }
                                    }
                                    //Set the new player to be allocated to the highest member of the lowestAllocatedPair
                                    i = players.IndexOf(lowestAllocatedPair) - 1;
                                    //Set the starting player that will be tested for allocation suitability to one rank lower than 
                                    //the opponent of the highest member of the allocated pair
                                    secondaryIndex = players.IndexOf(lowestAllocatedPair.CurrentOpponent) + 1;

                                    //Deallocate the lowest allocated pair as each other's opponent
                                    lowestAllocatedPair.CurrentOpponent.CurrentOpponent = null;
                                    lowestAllocatedPair.CurrentOpponent = null;

                                }
                            }
                        }
                    }
                }
                i++;
            }

            int newRound = GetLastRoundNo(_context) + 1;
            foreach (Player player in players)
            {
                if (players.IndexOf(player) < players.IndexOf(player.CurrentOpponent))
                {
                    RoundMatchup roundMatchup = new RoundMatchup
                    {
                        RoundNo = newRound,
                        PlayerOne = player,
                        PlayerTwo = player.CurrentOpponent
                    };

                    //allocates table for matchup
                   // roundMatchup.Table = AllocateTable(GetTables(player), AllocatedTables);

                    _context.Add(roundMatchup);
                }
            }

            foreach (Player player in players)
            {
                player.CurrentOpponent = null;
                _context.Update(player);
            }

            _context.SaveChanges();
        }

        public static void GenerateNextPairRound(TournamentDbContext _context)
        {
            int lastRound = GetLastRoundNo(_context);
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == lastRound).ToList();
            List<PlayerPair> playerPairs = new List<PlayerPair>();
            foreach (RoundMatchup roundMatchup in roundMatchups)
            {
                PlayerPair pair = new PlayerPair()
                {
                    First = roundMatchup.PlayerOne,
                    Second = roundMatchup.PlayerTwo
                };
                playerPairs.Add(pair);
            }

            //List<int> AllocatedTables = new List<int>(GetnoOfTables());
            int secondaryIndex = 0;
            int i = 0;

            while (i < playerPairs.Count)
            {
                //Skip this player if they are already allocated an opponent
                if (playerPairs[i].CurrentOpponent == null)
                {
                    if (secondaryIndex == 0) { secondaryIndex = i + 1; }
                    int s = 0;

                    for (s = secondaryIndex; s < playerPairs.Count; s++)
                    {
                        if (playerPairs[s].CurrentOpponent == null)
                        {

                            //Check if higher player has ever played lower player
                            var playerOneOpponents = PlayerActions.GetAllOpponents(playerPairs[i].First, _context);
                            var playerTwoOpponents = PlayerActions.GetAllOpponents(playerPairs[i].Second, _context);
                            var opponents = playerOneOpponents.Union(playerTwoOpponents);
                            var hasPlayed = false;
                            foreach (Player opponent in opponents)
                            {
                                if (playerPairs[s].First == opponent || playerPairs[s].Second == opponent) { hasPlayed = true; }
                            }
                            //If they have not played allocate them as opponents
                            if (hasPlayed == false)
                            {
                                playerPairs[i].CurrentOpponent = playerPairs[s];
                                playerPairs[s].CurrentOpponent = playerPairs[i];
                                secondaryIndex = 0;
                                break;
                            }

                            /**
                             * Following block is to deallocate the next lowest ranked allocated pair
                             **/
                            if (playerPairs.Where(p => p.CurrentOpponent == null).LastOrDefault() == playerPairs[s] && (playerPairs[i].CurrentOpponent == null))
                            //if (s == (players.Count - 1) && (players[i].CurrentOpponent == null))
                            {
                                if (i - 1 >= 0)
                                {
                                    //Set the lowestAllocatedPair to the highest ranked pair
                                    PlayerPair lowestAllocatedPair = playerPairs[0];
                                    //Iterate from the second highest ranked pair all the way to the pair ranked one higher than the player currently being matched
                                    for (int playerIndex = 1; playerIndex < i; playerIndex++)
                                    {
                                        //Assign the current player to the player being examined in the current iteration of the loop 
                                        PlayerPair currentPair = playerPairs[playerIndex];

                                        //Check that the current player has an opponent (if not, skip to the next iteration of the loop)
                                        if (currentPair.CurrentOpponent != null)
                                        {
                                            //Proceed if the current player's opponent has a higher rank than the current player
                                            if (playerPairs.IndexOf(currentPair.CurrentOpponent) < playerPairs.IndexOf(currentPair))
                                            {
                                                if (playerPairs.IndexOf(currentPair.CurrentOpponent) > playerPairs.IndexOf(lowestAllocatedPair))
                                                {
                                                    //Set lowestAllocatedPair to the current player's opponent 
                                                    //(The highest ranked member of the new lowest ranked allocated pair)
                                                    lowestAllocatedPair = currentPair.CurrentOpponent;
                                                }
                                            }
                                            //Proceed if the current player has a higher rank than their opponent
                                            else if (playerPairs.IndexOf(currentPair) < playerPairs.IndexOf(currentPair.CurrentOpponent))
                                            {
                                                //Proceed if the current player has a lower rank than the previous value of lowestAllocatedPair 
                                                if (playerPairs.IndexOf(currentPair) > playerPairs.IndexOf(lowestAllocatedPair))
                                                {
                                                    //Set lowestAllocatedPair to the current player 
                                                    //(The highest ranked member of the new lowest ranked allocated pair)
                                                    lowestAllocatedPair = currentPair;
                                                }
                                            }
                                        }
                                    }
                                    //Set the new player to be allocated to the highest member of the lowestAllocatedPair
                                    i = playerPairs.IndexOf(lowestAllocatedPair) - 1;
                                    //Set the starting player that will be tested for allocation suitability to one rank lower than 
                                    //the opponent of the highest member of the allocated pair
                                    secondaryIndex = playerPairs.IndexOf(lowestAllocatedPair.CurrentOpponent) + 1;

                                    //Deallocate the lowest allocated pair as each other's opponent
                                    lowestAllocatedPair.CurrentOpponent.CurrentOpponent = null;
                                    lowestAllocatedPair.CurrentOpponent = null;
                                }
                            }
                        }
                    }
                }
                i++;
            }
            int newRound = lastRound + 1;
            foreach (PlayerPair playerPair in playerPairs)
            {
                if (playerPairs.IndexOf(playerPair) < playerPairs.IndexOf(playerPair.CurrentOpponent))
                {
                    PairRoundMatchup roundMatchup = new PairRoundMatchup
                    {
                        RoundNo = newRound,
                        PlayerOne = playerPair.First,
                        PlayerTwo = playerPair.Second,
                        PlayerThree = playerPair.CurrentOpponent.First,
                        PlayerFour = playerPair.CurrentOpponent.Second
                    };

                    //allocates table for matchup
                    //roundMatchup.Table = AllocateTable(GetTables(player), AllocatedTables);

                    _context.Add(roundMatchup);
                }
            }

            _context.SaveChanges();
        }

        /**
         * 
         * Validation
         * 
         **/

        //Get the errors for all roundMatchups, including any players on a team with themself or versing themself, any players
        //who have played the same opponent more than once, any players who have not played in every round and any players who have
        //played twice in one round
        
        public static List<List<string>> GetRoundMatchupErrors(TournamentDbContext _context)
        {

            var roundMatchups = _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).OrderByDescending(r => r.PlayerOne.BattleScore).ToList();

            List<string> duplicatePlayers = new List<string>();
            List<string> duplicateOpponents = new List<string>();
            List<string> unallocatedPlayers = new List<string>();
            List<string> overallocatedPlayers = new List<string>();
            Dictionary<string, int> playerRoundCount = new Dictionary<string, int>();
            List<Player> players = _context.Players.ToList();
            foreach (Player player in players)
            {
                playerRoundCount.Add(player.Name, 0);
            }

            List<String> tempDuplicatePlayers = new List<string>();
            foreach (RoundMatchup roundMatchup in roundMatchups)
            {
                if (roundMatchup is PairRoundMatchup)
                {
                    var pairRoundMatchup = roundMatchup as PairRoundMatchup;
                    // Check if PlayerOne is on a team with themself
                    if (pairRoundMatchup.PlayerOne == pairRoundMatchup.PlayerTwo)
                    {
                        duplicatePlayers.Add(pairRoundMatchup.PlayerOne.Name + " is on a team with themself in round " + pairRoundMatchup.RoundNo);
                    }
                    // Check if PlayerOne is playing themself
                    if (pairRoundMatchup.PlayerOne == pairRoundMatchup.PlayerThree || pairRoundMatchup.PlayerOne == pairRoundMatchup.PlayerFour)
                    {
                        duplicatePlayers.Add(pairRoundMatchup.PlayerOne.Name + " is playing themself in round " + pairRoundMatchup.RoundNo);
                    }
                    //Check if PlayerTwo is playing themself
                    if (pairRoundMatchup.PlayerTwo == pairRoundMatchup.PlayerThree || pairRoundMatchup.PlayerTwo == pairRoundMatchup.PlayerFour)
                    {
                        duplicatePlayers.Add(pairRoundMatchup.PlayerTwo.Name + " is playing themself in round " + pairRoundMatchup.RoundNo);
                    }
                    //Check if PlayerThree is on a team with themself
                    if (pairRoundMatchup.PlayerThree == pairRoundMatchup.PlayerFour)
                    {
                        duplicatePlayers.Add(pairRoundMatchup.PlayerThree.Name + " is on a team with themself in round " + pairRoundMatchup.RoundNo);
                    }
                }
                //Check if there are any players versing themselves
                else if (!(roundMatchup is PairRoundMatchup))
                {
                    if (roundMatchup.PlayerOne == roundMatchup.PlayerTwo)
                    {
                        duplicatePlayers.Add(roundMatchup.PlayerOne.Name + " is playing themself in round " + roundMatchup.RoundNo);
                    }
                }
                //Check if there are players who either do not play at all or play more than once
                foreach (Player player in _context.Players)
                {
                    if (player.Id == roundMatchup.PlayerOne.Id)
                    {
                        playerRoundCount[player.Name] += 1;
                    }
                    if (player.Id == roundMatchup.PlayerTwo.Id)
                    {
                        playerRoundCount[player.Name] += 1;
                    }
                    if (roundMatchup is PairRoundMatchup)
                    {
                        PairRoundMatchup pairRoundMatchup = roundMatchup as PairRoundMatchup;
                        if (player.Id == pairRoundMatchup.PlayerThree.Id)
                        {
                            playerRoundCount[player.Name] += 1;
                        }
                        if (player.Id == pairRoundMatchup.PlayerFour.Id)
                        {
                            playerRoundCount[player.Name] += 1;
                        }
                    }
                }
            }

            int roundCount = GetLastRoundNo(_context);
            foreach (KeyValuePair<string, int> entry in playerRoundCount)
            {
                if (entry.Value > roundCount)
                {
                    overallocatedPlayers.Add(entry.Key + " has been allocated to " + entry.Value + " matchups with only " + roundCount + " rounds played");
                }
                if (entry.Value < roundCount) { unallocatedPlayers.Add(entry.Key + " has only been allocated to " + entry.Value + " matchup/s when " + roundCount + " have been played"); }
            }

            Dictionary<Player, List<string>> duplicateOpponentsDictionary = new Dictionary<Player, List<string>>();
            foreach (Player player in players)
            {
                List<Player> opponents = PlayerActions.GetAllOpponents(player, _context);
                List<string> duplicateNames = opponents
                    .GroupBy(i => i)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key.Name)
                    .ToList();
                duplicateOpponentsDictionary[player] = duplicateNames;
            }

            foreach (var duplicateOpponentSet in duplicateOpponentsDictionary)
            {
                string duplicateOpponentFormattedNameList = string.Join(", ", duplicateOpponentSet.Value);
                if (duplicateOpponentSet.Value.Count != 0) { duplicateOpponents.Add(duplicateOpponentSet.Key.Name + " has played " + duplicateOpponentFormattedNameList + " at least twice");  }
            }

            var errors = new List<List<string>>() {
                duplicatePlayers,
                duplicateOpponents,
                overallocatedPlayers,
                unallocatedPlayers
            };

            return (errors);
        }
    }
}
