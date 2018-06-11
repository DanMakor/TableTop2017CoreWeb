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
        public static Boolean GenerateNextRound(TournamentDbContext _context)
        {
            List<Player> players = _context.Players.Where(p => p.Active && p.Bye == false && p.Name != "Bye").OrderByDescending(p => p.BattleScore).ToList();

            List<int> AllocatedTables = new List<int>(GetNoOfTables(_context));

            Boolean error = false;
            int secondaryIndex = 0;
            int i = 0;
            while (i < players.Count && i >= 0)
            {
                //Skip this player if they are already allocated an opponent
                if (players[i].CurrentOpponent == null)
                {
                    if (secondaryIndex == 0) { secondaryIndex = i + 1; }
                    int s = 0;

                    for (s = secondaryIndex; s < players.Count; s++)
                    {
                        //If there are no more unique matchups exit
                        if (i == -1) {
                            i = -2;
                            error = true;
                            break;
                        }
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

                                    //If there are no more unique matchups exit
                                    if (i == -1)
                                    {
                                        i = -2;
                                        error = true;
                                        break;
                                    }
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

            int newRoundNo = GetLastRoundNo(_context) + 1;
            foreach (Player player in players)
            {
                if (players.IndexOf(player) < players.IndexOf(player.CurrentOpponent))
                {
                    var roundMatchup = new RoundMatchup
                    {
                        RoundNo = newRoundNo,
                        PlayerOne = player,
                        PlayerTwo = player.CurrentOpponent
                    };

                    //allocates table for matchup

                    roundMatchup.Table = AllocateTable(GetTables(player, _context), AllocatedTables, _context);

                    _context.Add(roundMatchup);
                }
                //If there are no more unique matchups
                if (error)
                {
                    //RoundMatchup will hold each pair of players closest to each other in BattleScore
                    //E.G (List ordered by BattleScore) players[0] vs players[1] | players[2] vs players[3]
                    if (player.CurrentOpponent == null && players[players.IndexOf(player) + 1] != null)
                    {
                        player.CurrentOpponent = players[players.IndexOf(player) + 1];
                        player.CurrentOpponent.CurrentOpponent = player;
                        var roundMatchup = new RoundMatchup
                        {
                            RoundNo = newRoundNo,
                            PlayerOne = player,
                            PlayerTwo = player.CurrentOpponent
                        };


                        roundMatchup.Table = AllocateTable(GetTables(player, _context), AllocatedTables, _context);

                        _context.Add(roundMatchup);
                    }
                }

            }

            CreateByeRounds(_context.Players.Where(p => p.Active && p.Bye && p.Name != "Bye").ToList(), newRoundNo, _context);

            foreach (Player player in players)
            {
                player.CurrentOpponent = null;
                _context.Update(player);
            }

            _context.SaveChanges();

            if (error)
                return false;
            return true;
        }

        public static Boolean GenerateNextPairRound(TournamentDbContext _context)
        {
            Boolean error = false;
            int lastRound = GetLastRoundNo(_context);
            var roundMatchups = _context.RoundMatchups.Where(r => !(r is PairRoundMatchup)).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == lastRound).ToList();
            var pairRoundMatchups = _context.PairRoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).Where(r => r.RoundNo == lastRound).ToList();
            List<Player> activePlayers = _context.Players.Where(p => p.Active && p.Bye == false).ToList();
            List<Player> activeByePlayers = new List<Player>();
            List<PlayerPair> playerPairs = new List<PlayerPair>();
            roundMatchups = roundMatchups.Union(pairRoundMatchups).ToList();

            if (roundMatchups != null)
            {
                Tuple<List<PlayerPair>, List<Player>> playerPairsAndByes = (RoundMatchupActions.GetPlayerPairs(roundMatchups, activePlayers));
                playerPairs = playerPairsAndByes.Item1;
                activeByePlayers = playerPairsAndByes.Item2;
            } else
            {
                error = true;
            }

            //List<int> AllocatedTables = new List<int>(GetnoOfTables());
            int secondaryIndex = 0;
            int i = 0;

            while (i < playerPairs.Count && i >= 0)
            {
                //Skip this player if they are already allocated an opponent
                if (playerPairs[i].CurrentOpponent == null)
                {
                    if (secondaryIndex == 0) { secondaryIndex = i + 1; }
                    int s = 0;

                    for (s = secondaryIndex; s < playerPairs.Count; s++)
                    {
                        //If there are no more unique matchups, exit
                        if (i == -1)
                        {
                            i = -2;
                            error = true;
                            break;
                        }
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

                                    //If there are no more unique matchups, exit
                                    if (i == -1)
                                    {
                                        i = -2;
                                        error = true;
                                        break;
                                    }

                                    //Set the starting player that will be tested for allocation suitability to one rank lower than 
                                    //the opponent of the highest member of the allocated pair
                                    secondaryIndex = playerPairs.IndexOf(lowestAllocatedPair.CurrentOpponent) + 1;

                                    //Deallocate the lowest allocated pair as each other's opponent
                                    lowestAllocatedPair.CurrentOpponent.CurrentOpponent = null;
                                    lowestAllocatedPair.CurrentOpponent = null;
                                }
                                else
                                {
                                    i = -2;
                                    error = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                i++;
            }
            int newRoundNo = lastRound + 1;
            foreach (PlayerPair playerPair in playerPairs)
            {
                if (playerPairs.IndexOf(playerPair) < playerPairs.IndexOf(playerPair.CurrentOpponent))
                {
                    var roundMatchup = new PairRoundMatchup
                    {
                        RoundNo = newRoundNo,
                        PlayerOne = playerPair.First,
                        PlayerTwo = playerPair.Second,
                        PlayerThree = playerPair.CurrentOpponent.First,
                        PlayerFour = playerPair.CurrentOpponent.Second
                    };

                    //allocates table for matchup
                    //roundMatchup.Table = AllocateTable(GetTables(player), AllocatedTables);

                    _context.Add(roundMatchup);
                }
                //If there are no more unique matchups
                if (error)
                {
                    //RoundMatchup will hold random pair matchups where teammates were opponents last round unless their opponent is no longer active or has a bye
                    if (playerPair.CurrentOpponent == null && playerPairs[playerPairs.IndexOf(playerPair) + 1] != null)
                    {
                        playerPair.CurrentOpponent = playerPairs[playerPairs.IndexOf(playerPair) + 1];
                        playerPair.CurrentOpponent.CurrentOpponent = playerPair;
                        var roundMatchup = new PairRoundMatchup
                        {
                            RoundNo = newRoundNo,
                            PlayerOne = playerPair.First,
                            PlayerTwo = playerPair.Second,
                            PlayerThree = playerPair.CurrentOpponent.First,
                            PlayerFour = playerPair.CurrentOpponent.Second
                        };
                        _context.Add(roundMatchup);
                    }
                }
            }

            CreateByeRounds(activeByePlayers, newRoundNo, _context);

            _context.SaveChanges();

            if (error)
                return false;
            return true;
        }

        public static Tuple<List<PlayerPair>, List<Player>> GetPlayerPairs(List<RoundMatchup> roundMatchups, List<Player> activePlayers) {

            List<Player> activeUnallocatedPlayers = new List<Player>();
            List<Player> activeByePlayers = new List<Player>();
            List<Player> lastRoundPlayers = new List<Player>();
            List<PlayerPair> playerPairs = new List<PlayerPair>();

            foreach (RoundMatchup roundMatchup in roundMatchups)
            {
                //If last round was a bye
                if (roundMatchup.PlayerTwo == null)
                {
                    //If the player who played in the bye matchup is active add them to the activeUnallocatedPlayers as they have no previous opponent to be matched against
                    if (roundMatchup.PlayerOne.Active == true)
                    {
                        activeUnallocatedPlayers.Add(roundMatchup.PlayerOne);
                    }
                }
                else
                {
                    //Populate a list full of all the players that played last round
                    lastRoundPlayers.Add(roundMatchup.PlayerOne);
                    lastRoundPlayers.Add(roundMatchup.PlayerTwo);
                    if (!(roundMatchup is PairRoundMatchup))
                    {
                        if (roundMatchup.PlayerOne.Active && roundMatchup.PlayerOne.Bye == false && roundMatchup.PlayerTwo.Active && roundMatchup.PlayerTwo.Bye == false)
                        {
                            playerPairs.Add(new PlayerPair()
                            {
                                First = roundMatchup.PlayerOne,
                                Second = roundMatchup.PlayerTwo
                            });
                        }

                        //If PlayerOne from this roundmatchup last round is inactive, add PlayerTwo to the unallocated players list
                        else if (roundMatchup.PlayerOne.Active == false || roundMatchup.PlayerOne.Bye == true && roundMatchup.PlayerTwo.Active && roundMatchup.PlayerTwo.Bye == false)
                        {
                            activeUnallocatedPlayers.Add(roundMatchup.PlayerTwo);
                        }

                        //If PlayerTwo from this roundmatchup last round is inactive, add PlayerOne to the unallocated players list
                        else if (roundMatchup.PlayerTwo.Active == false || roundMatchup.PlayerTwo.Bye == true && roundMatchup.PlayerOne.Active && roundMatchup.PlayerOne.Bye == false)
                        {
                            activeUnallocatedPlayers.Add(roundMatchup.PlayerOne);
                        }
                        //If PlayerOne or PlayerTwo have a bye and are active, add both to the unallocated players list (will filter which player has a bye later)
                        else if ((roundMatchup.PlayerOne.Bye && roundMatchup.PlayerOne.Active) || (roundMatchup.PlayerTwo.Bye && roundMatchup.PlayerTwo.Active))
                        {
                            activeUnallocatedPlayers.Add(roundMatchup.PlayerOne);
                            activeUnallocatedPlayers.Add(roundMatchup.PlayerTwo);
                        }
                    }
                    else
                    {
                        var pairRoundMatchup = roundMatchup as PairRoundMatchup;
                        //Populate a list full of all the players that played last round

                        lastRoundPlayers.Add(pairRoundMatchup.PlayerThree);
                        lastRoundPlayers.Add(pairRoundMatchup.PlayerFour);
                        lastRoundPlayers.Add(pairRoundMatchup.PlayerOne);
                        lastRoundPlayers.Add(pairRoundMatchup.PlayerTwo);
                        
                        if (pairRoundMatchup.PlayerOne.Active && pairRoundMatchup.PlayerOne.Bye == false && pairRoundMatchup.PlayerThree.Active && pairRoundMatchup.PlayerThree.Bye == false)
                        {
                            playerPairs.Add(new PlayerPair()
                            {
                                First = pairRoundMatchup.PlayerOne,
                                Second = pairRoundMatchup.PlayerThree
                            });
                        }

                        //If PlayerOne from this pairRoundMatchup last round is inactive, add PlayerTwo to the unallocated players list
                        else if (pairRoundMatchup.PlayerOne.Active == false || pairRoundMatchup.PlayerOne.Bye == true && pairRoundMatchup.PlayerThree.Active && pairRoundMatchup.PlayerThree.Bye == false)
                        {
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerThree);
                        }

                        //If PlayerTwo from this pairRoundMatchup last round is inactive, add PlayerOne to the unallocated players list
                        else if (pairRoundMatchup.PlayerThree.Active == false || pairRoundMatchup.PlayerThree.Bye == true && pairRoundMatchup.PlayerOne.Active && pairRoundMatchup.PlayerOne.Bye == false)
                        {
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerOne);
                        }
                        //If PlayerOne or PlayerTwo have a bye and are active, add both to the unallocated players list (will filter which player has a bye later)
                        else if ((pairRoundMatchup.PlayerOne.Bye && pairRoundMatchup.PlayerOne.Active) || (pairRoundMatchup.PlayerThree.Bye && pairRoundMatchup.PlayerThree.Active))
                        {
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerOne);
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerThree);
                        }

                        if (pairRoundMatchup.PlayerTwo.Active && pairRoundMatchup.PlayerTwo.Bye == false && pairRoundMatchup.PlayerFour.Active && pairRoundMatchup.PlayerFour.Bye == false)
                        {
                            playerPairs.Add(new PlayerPair()
                            {
                                First = pairRoundMatchup.PlayerTwo,
                                Second = pairRoundMatchup.PlayerFour
                            });
                        }
                        else if (pairRoundMatchup.PlayerTwo.Active == false || pairRoundMatchup.PlayerTwo.Bye == true && pairRoundMatchup.PlayerFour.Active && pairRoundMatchup.PlayerFour.Bye == false)
                        {
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerFour);
                        }
                        //If PlayerOne from this roundmatchup last round is inactive, add PlayerOne to the unallocated players list
                        else if (pairRoundMatchup.PlayerFour.Active == false || pairRoundMatchup.PlayerFour.Bye == true && pairRoundMatchup.PlayerTwo.Active && pairRoundMatchup.PlayerTwo.Bye == false)
                        {
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerTwo);
                        }
                        //If PlayerThree or PlayerFour have a bye and are active, add both to the unallocated players list (will filter which player has a bye later)
                        else if ((pairRoundMatchup.PlayerTwo.Bye && pairRoundMatchup.PlayerTwo.Active) || (pairRoundMatchup.PlayerFour.Bye && pairRoundMatchup.PlayerFour.Active))
                        {
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerTwo);
                            activeUnallocatedPlayers.Add(pairRoundMatchup.PlayerFour);
                        }
                    }
                }
            }
            //Currently we are matching pairRound teammates from the prevoius round as teammates again
            //Abstract out the checking if the players are active

            foreach (Player activePlayer in activePlayers)
            {
                Boolean playedLastRound = false;
                foreach (Player lastRoundPlayer in lastRoundPlayers)
                {
                    if (activePlayer == lastRoundPlayer)
                        playedLastRound = true;
                }
                if (playedLastRound == false)
                    activeUnallocatedPlayers.Add(activePlayer);
            }

            //Separate the unallocated players that have a bye and those that don't
            activeByePlayers = activeUnallocatedPlayers.Where(p => p.Bye && p.Active).ToList();
            activeUnallocatedPlayers.RemoveAll(p => p.Bye);

            //Loop through the active unallocated players
            foreach (Player activeUnallocatedPlayer in activeUnallocatedPlayers)
            {
                if ((activeUnallocatedPlayers.IndexOf(activeUnallocatedPlayer) + 1) < activeUnallocatedPlayers.Count())
                {
                    playerPairs.Add(new PlayerPair()
                    {
                        First = activeUnallocatedPlayer,
                        Second = activeUnallocatedPlayers[activeUnallocatedPlayers.IndexOf(activeUnallocatedPlayer) + 1]
                    });
                } else
                {
                    //errorMessage = "Odd number of players in the pair matchups" 
                }
            }

            return Tuple.Create(playerPairs, activeByePlayers);
        }

        public static void CreateByeRounds(List<Player> activeByePlayers, int roundNo, TournamentDbContext _context)
        {
            foreach (Player activeByePlayer in activeByePlayers)
            {
                _context.Add(new PairRoundMatchup()
                {
                    RoundNo = roundNo,
                    PlayerOne = activeByePlayer
                });
            }
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

            var preSortedRoundMatchups = _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => !(r is PairRoundMatchup) && r.PlayerTwo.Name != "Bye").ToList();
            var preSortedPairRoundMatchups = _context.RoundMatchups.OfType<PairRoundMatchup>().Include(r => r.PlayerOne).Include(r => r.PlayerThree).Where(r => r.PlayerThree.Name != "Bye").ToList();
            List<RoundMatchup> roundMatchups = preSortedRoundMatchups.Union(preSortedPairRoundMatchups).OrderBy(r => r.RoundNo).ToList();

            List<string> duplicatePlayers = new List<string>();
            List<string> duplicateOpponents = new List<string>();
            List<string> unallocatedPlayers = new List<string>();
            List<string> overallocatedPlayers = new List<string>();
            Dictionary<Player, int> playerRoundCount = new Dictionary<Player, int>();
            List<Player> players = _context.Players.ToList();
            foreach (Player player in players)
            {
                playerRoundCount.Add(player, 0);
            }

            foreach (RoundMatchup roundMatchup in roundMatchups)
            {

                //Check if there are players who either do not play at all or play more than once
                foreach (Player player in players)
                {
                    if (player.Id == roundMatchup.PlayerOne.Id)
                    {
                        playerRoundCount[player] += 1;
                    }
                    //Only check playerTwo if they are not null (Will be null in a bye round)
                    if (roundMatchup.PlayerTwo != null)
                    {
                        //Increment the amount of matchups player two has been a part of if they are not the same player as player one
                        if (player.Id == roundMatchup.PlayerTwo.Id && roundMatchup.PlayerOne != roundMatchup.PlayerTwo)
                        {
                            playerRoundCount[player] += 1;
                        }
                    }
                    if (roundMatchup is PairRoundMatchup)
                    {
                        PairRoundMatchup pairRoundMatchup = roundMatchup as PairRoundMatchup;
                        //Only check playerThree and playerFour if they are not null (Will be null in a bye round)
                        if (pairRoundMatchup.PlayerThree != null && pairRoundMatchup.PlayerFour != null)
                        {
                            //Increment the amount of matchups player three has played of if they are not the same player as player one or two
                            if (player.Id == pairRoundMatchup.PlayerThree.Id && pairRoundMatchup.PlayerThree != pairRoundMatchup.PlayerTwo && pairRoundMatchup.PlayerThree != pairRoundMatchup.PlayerOne)
                            {
                                playerRoundCount[player] += 1;
                            }
                            //Increment the amount of matchups player four has played if they are not the same player as player one, two or three
                            if (player.Id == pairRoundMatchup.PlayerFour.Id && pairRoundMatchup.PlayerFour != pairRoundMatchup.PlayerThree && pairRoundMatchup.PlayerFour != pairRoundMatchup.PlayerTwo && pairRoundMatchup.PlayerFour != pairRoundMatchup.PlayerOne)
                            {
                                playerRoundCount[player] += 1;
                            }
                        }
                    }
                }
            }

            //Is set up so that if a player is allocated to themselves playerRoundCount is only incremented once. 
            //It will warn that a player has not played enough rounds, even if they are versing themself in a round. 
            //It will not warn that a player has played too many rounds if they have been allocated one too many times but
            //are versing themself in a round.

            int roundCount = GetLastRoundNo(_context);
            foreach (KeyValuePair<Player, int> entry in playerRoundCount)
            {
                if (entry.Value > roundCount)
                {
                    overallocatedPlayers.Add(entry.Key.Name + " has been allocated " + entry.Value + " times with only " + roundCount + " rounds played");
                }
                if (entry.Value < roundCount)
                {
                    unallocatedPlayers.Add(entry.Key.Name + " has only been allocated to " + entry.Value + " matchup/s when " + roundCount + " have been played");
                }
            }

            Dictionary<Player, List<Player>> duplicateOpponentsDictionary = PlayerActions.GetPreviouslyPlayedOpponentClashes(_context);
            foreach (var duplicateOpponentSet in duplicateOpponentsDictionary)
            {
                List<string> duplicateOpponentNameList = new List<string>(); 
                foreach (Player duplicateOpponent in duplicateOpponentSet.Value)
                {
                    duplicateOpponentNameList.Add(duplicateOpponent.Name);
                }
                string duplicateOpponentFormattedNameList = string.Join(", ", duplicateOpponentNameList);
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

        //Returns a table to be assigned to a matchup. Also keeps a record of allocated tables for round. 
        public static int AllocateTable(List<int> tables, List<int> allocated, TournamentDbContext _context)
        {
            var isAvailable = true;

            foreach (int tableNo in tables)
            {
                isAvailable = true;
                for (int i = 0; i < allocated.Count; i++)
                {

                    if (allocated[i] == tableNo)
                    {

                        isAvailable = false;
                    }
                }
                if (isAvailable == true)
                {
                    allocated.Add(tableNo);
                    return tableNo;
                }

            }
            //if unble to allocate a table that has not been played on then a random table will be assigned
            if (allocated.Count > 0)
            {

                // Debug.WriteLine("THIS IS THE ALLOCATED.Count----))*  \n" + allocated.Count);


                for (int i = 1; i <= GetNoOfTables(_context); i++)
                {
                    isAvailable = true;
                    foreach (int table in allocated)
                    {

                        if (i == table)
                        {

                            isAvailable = false;
                        }
                    }
                    if (isAvailable == true)
                    {
                        allocated.Add(i);
                        return i;
                    }
                }



            }
            else
            {

                Random r = new Random();
                int randomTable = r.Next(1, 12);
                int NumOfTables = GetNoOfTables(_context);
                if (NumOfTables >= 1)
                {
                    randomTable = r.Next(1, NumOfTables);
                }
                allocated.Add(randomTable);
                return randomTable;

            }

            //At the moment this is just to return a number when there are no more possible combinations for players and tables
            return 999;

        }
        public static int GetNoOfTables(TournamentDbContext _context)
        {
            if (_context.Rounds.Count() > 0)
            {

                return _context.Rounds.Last().NoTableTops;
            }


            return 0;
        }
        //returns a list of available tables that the player has not played on
        public static List<int> GetTables(Player currentPlayer, TournamentDbContext _context)
        {

            // List<int> tables = _context.RoundMatchups.Where(a => a.PlayerOne == currentPlayer || a.PlayerOne.CurrentOpponent == currentPlayer).Select(a => a.Table).ToList();
            List<int> tables = new List<int>();
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.ToList();
            int currentRound = 1;
            if (_context.RoundMatchups.LastOrDefault() != null)
            {
                currentRound = _context.RoundMatchups.Last().RoundNo + 1;
            }
            //adds all tables to list
            for (int j = 1; j <= GetNoOfTables(_context); j++)
            {
                tables.Add(j);
            }
            if (currentRound < 2)
                return tables;
            else
            {
                //List<int> temp = _context.RoundMatchups.Where(a => a.PlayerOne == currentPlayer || a.PlayerTwo == currentPlayer).Select(a => a.Table).ToList();
                // tables = (List<int>)tables.Except(temp); // returns all the firms except those in _context.RoundMatchups.Where(Ect..)
                //*
                foreach (var roundMatchup in roundMatchups)
                {
                    if (roundMatchup.PlayerOne == currentPlayer || roundMatchup.PlayerTwo == currentPlayer || currentPlayer.CurrentOpponent == roundMatchup.PlayerOne || currentPlayer.CurrentOpponent == roundMatchup.PlayerTwo)
                    {
                        tables.Remove(roundMatchup.Table);
                    }
                }
                //*/

            }



            //Where(r => r.roundNo == currentRound);
            return tables;
        }

    }
}

//No longer needed due to on submission validation of any duplication
//if (roundMatchup.PlayerTwo != null)
//                {
//                    if (roundMatchup is PairRoundMatchup)
//                    {
//                        var pairRoundMatchup = roundMatchup as PairRoundMatchup;
//                        // Check if PlayerOne is on a team with themself
//                        if (pairRoundMatchup.PlayerOne == pairRoundMatchup.PlayerTwo)
//                        {
//                            duplicatePlayers.Add(pairRoundMatchup.PlayerOne.Name + " is on a team with themself in round " + pairRoundMatchup.RoundNo);
//                        }
//                        // Check if PlayerOne is playing themself
//                        if (pairRoundMatchup.PlayerOne == pairRoundMatchup.PlayerThree || pairRoundMatchup.PlayerOne == pairRoundMatchup.PlayerFour)
//                        {
//                            duplicatePlayers.Add(pairRoundMatchup.PlayerOne.Name + " is playing themself in round " + pairRoundMatchup.RoundNo);
//                        }
//                        //Check if PlayerTwo is playing themself
//                        if (pairRoundMatchup.PlayerTwo == pairRoundMatchup.PlayerThree || pairRoundMatchup.PlayerTwo == pairRoundMatchup.PlayerFour)
//                        {
//                            duplicatePlayers.Add(pairRoundMatchup.PlayerTwo.Name + " is playing themself in round " + pairRoundMatchup.RoundNo);
//                        }
//                        //Check if PlayerThree is on a team with themself
//                        if (pairRoundMatchup.PlayerThree == pairRoundMatchup.PlayerFour)
//                        {
//                            duplicatePlayers.Add(pairRoundMatchup.PlayerThree.Name + " is on a team with themself in round " + pairRoundMatchup.RoundNo);
//                        }
//                    }
//                    //Check if there are any players versing themselves
//                    else if (!(roundMatchup is PairRoundMatchup))
//                    {
//                        if (roundMatchup.PlayerOne == roundMatchup.PlayerTwo)
//                        {
//                            duplicatePlayers.Add(roundMatchup.PlayerOne.Name + " is playing themself in round " + roundMatchup.RoundNo);
//                        }
//                    }
//                }