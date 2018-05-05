using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using Microsoft.EntityFrameworkCore;
using TableTop2017CoreWeb.Data;
using TableTop2017CoreWeb.Models;



namespace TableTop2017CoreWeb.Controllers
{

    public class RoundMatchupsController : Controller
    {

        private readonly TournamentDbContext _context;

        public RoundMatchupsController(TournamentDbContext context)
        {
            _context = context;
        }

        public void Database()
        {
            List<RoundMatchups> roundmatchups = _context.RoundMatchups.ToList();
            foreach(RoundMatchups roundmatchup in roundmatchups)
            {
                _context.Remove(roundmatchup);
            }
            _context.SaveChanges();
            SetAllPlayerBattleScores();
        }

        // GET: RoundMatchups
        public async Task<IActionResult> Index()
        {
            int currentRound = await GetCurrentRound();
            List<RoundMatchups> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();
            return View(roundMatchups);
        }

        // GET: AdminMatchups
        public async Task<IActionResult> Admin()
        {
            int currentRound = await GetCurrentRound();
            List<RoundMatchups> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();
            TempData["DuplicatePlayers"] = null;
            TempData["OverallocatedPlayers"] = null;
            TempData["UnallocatedPlayers"] = null;

            return View(roundMatchups);
        }

        // GET: RoundMatchups Results
        public async Task<IActionResult> Results()
        {
            int currentRound = await GetCurrentRound();
            List<RoundMatchups> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();
            return View(roundMatchups);
        }

        // GET: Allrounds
        public async Task<IActionResult> AllRounds()
        {
            List<RoundMatchups> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToListAsync();
            TempData["DuplicateOpponents"] = null;
            return View(roundMatchups);
        }

        //GET: AllRoundsEdit
        public async Task<IActionResult> AllRoundsEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var roundMatchup = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).SingleOrDefaultAsync(m => m.Id == id);
            return View(roundMatchup);
        }

        //Update the Player Battlescores before redirecting to the player page
        public ActionResult UpdateBattleScores()
        {
            SetAllPlayerBattleScores();
            return RedirectToAction("Index", "Players");
        }
        //Update the Player Battlescores before redirecting to the player page
        public ActionResult UpdateBattleScoresContinue(String page)
        {
            SetAllPlayerBattleScores();
            GenerateRoundMatchupsAlgorithm();
            return RedirectToAction(page, "RoundMatchups");
        }

        //GET: ResultsEdit
        public async Task<IActionResult> ResultsEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchup = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).SingleOrDefaultAsync(m => m.Id == id);
            return View(roundMatchup);
        }

        //POST: RoundMatchups Results

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResultsEdit(int id, [Bind("Id,RoundNo,PlayerOneId,PlayerTwoId,PlayerOneBattleScore,PlayerTwoBattleScore,PlayerOneSportsmanshipScore,PlayerTwoSportsmanshipScore,Table")] RoundMatchups roundMatchup)
        {
            if (id != roundMatchup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roundMatchup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoundMatchupsExists(roundMatchup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Results));
            }
            return View(roundMatchup);
        }

        //Generate the RoundMatchups 
        public ActionResult GenerateRoundMatchups()
        {
            GenerateRoundMatchupsAlgorithm();
            return RedirectToAction("Admin");
        }

        // GET: RoundMatchups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.Table)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (roundMatchups == null)
            {
                return NotFound();
            }

            return View(roundMatchups);
        }

        // GET: RoundMatchups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RoundMatchups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,roundNo,battlePoints,sportsmanshipPoints,table")] RoundMatchups roundMatchups)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roundMatchups);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roundMatchups);
        }

        //GET: AdminEdit
        public async Task<IActionResult> AdminEdit(int? id)
        {
            var aevm = new AdminEditViewModel();

            if (id == null)
            {
                return NotFound();
            }

            var roundMatchup = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).SingleOrDefaultAsync(m => m.Id == id);
            aevm.RoundMatchup = new AdminEditRoundMatchupsViewModel
            {
                Id = roundMatchup.Id,
                PlayerOneId = roundMatchup.PlayerOne.Id,
                PlayerTwoId = roundMatchup.PlayerTwo.Id,
                TableNo = roundMatchup.Table
            };
            if (aevm.RoundMatchup == null)
            {
                return NotFound();
            }
            aevm.Players = await _context.Players.ToListAsync();
            return View(aevm);
        }

        //POST: AdminEdit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEdit(int id, [Bind("Id,PlayerOneId,PlayerTwoId,TableNo")] AdminEditRoundMatchupsViewModel roundMatchup)
        {
            if (id != roundMatchup.Id)
            {
                return NotFound();
            }

            RoundMatchups updatedRoundMatchup = _context.RoundMatchups.Find(id);
            updatedRoundMatchup.PlayerOne = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerOneId);
            updatedRoundMatchup.PlayerTwo = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerTwoId);
            updatedRoundMatchup.Table = roundMatchup.TableNo;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedRoundMatchup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoundMatchupsExists(roundMatchup.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Admin));
            }
            return View(roundMatchup);
        }

        //Validate The Matchups after they have been changed on the admin page 
        public async Task<IActionResult> ValidateMatchups()
        {
            int currentRound = await GetCurrentRound();
            List<RoundMatchups> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();

            List<String> duplicatePlayers = new List<String>();
            List<String> unallocatedPlayers = new List<string>();
            List<String> overallocatedPlayers = new List<String>();
            Dictionary<string, int> playerRoundCount = new Dictionary<string, int>();
            foreach (Player player in _context.Players.ToList())
                {
                    playerRoundCount.Add(player.Name, 0);
                }

            foreach (RoundMatchups roundMatchup in roundMatchups)
            {
                //Check if there are any players versing themselves
                if (roundMatchup.PlayerOne == roundMatchup.PlayerTwo)
                {
                    Boolean stored = false;
                    foreach (var player in duplicatePlayers)
                    {
                        if (player == roundMatchup.PlayerOne.Name)
                        {
                            stored = true;
                        }
                    }
                    if (stored == false)
                    {
                        duplicatePlayers.Add(roundMatchup.PlayerOne.Name);
                    }
                }
                //Check if there are players who either do not play at all or play more than once
                foreach (Player player in _context.Players) { 
                    if (player.Id == roundMatchup.PlayerOne.Id)
                    {
                        playerRoundCount[player.Name] += 1;
                    }
                    if (player.Id == roundMatchup.PlayerTwo.Id)
                    {
                        playerRoundCount[player.Name] += 1;

                    }
                }
            }

            foreach (KeyValuePair<string, int> entry in playerRoundCount)
            {
                if (entry.Value > 1) { overallocatedPlayers.Add(entry.Key); }
                if (entry.Value < 1) { unallocatedPlayers.Add(entry.Key); }
            }

            if (duplicatePlayers.Count() > 0) { TempData["DuplicatePlayers"] = duplicatePlayers; } else { TempData["DuplicatePlayers"] = null; }
            if (overallocatedPlayers.Count() > 0) { TempData["OverallocatedPlayers"] = overallocatedPlayers; } else { TempData["OverallocatedPlayers"] = null; }
            if (unallocatedPlayers.Count() > 0) { TempData["UnallocatedPlayers"] = unallocatedPlayers; } else { TempData["UnallocatedPlayers"] = null; }

            if (duplicatePlayers.Count == 0 && overallocatedPlayers.Count == 0 && unallocatedPlayers.Count == 0)
            {
                TempData["Status"] = "No issues!";
            }

            return View(nameof(Admin), roundMatchups);
        }
        
        //Reset the matchups 
        public async Task<IActionResult> ResetMatchups()
        {
            int roundNo = await GetCurrentRound();
            foreach (RoundMatchups roundMatchup in _context.RoundMatchups.Where(r => r.RoundNo == roundNo).ToList())
            {
                _context.Remove(roundMatchup);
            }
            await _context.SaveChangesAsync();
            GenerateRoundMatchupsAlgorithm();
            return RedirectToAction(nameof(Admin));
        }

        // GET: RoundMatchups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.Table).SingleOrDefaultAsync(m => m.Id == id);
            if (roundMatchups == null)
            {
                return NotFound();
            }
            return View(roundMatchups);
        }

        // POST: RoundMatchups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoundNo,PlayerOneBattleScore,PlayerTwoBattleScore,PlayerOneSportsmanshipPoints,PlayerTwoSportsmanshipPoints,Table")] RoundMatchups roundMatchups)
        {
            if (id != roundMatchups.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roundMatchups);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoundMatchupsExists(roundMatchups.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(roundMatchups);
        }

        // GET: RoundMatchups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.Table)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (roundMatchups == null)
            {
                return NotFound();
            }

            return View(roundMatchups);
        }

        // POST: RoundMatchups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roundMatchups = await _context.RoundMatchups.SingleOrDefaultAsync(m => m.Id == id);
            _context.RoundMatchups.Remove(roundMatchups);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoundMatchupsExists(int id)
        {
            return _context.RoundMatchups.Any(e => e.Id == id);
        }
        public List<Player> getAllOpponents(Player player)
        {
            List<Player> opponents = new List<Player>();
            List<RoundMatchups> roundMatchups = _context.RoundMatchups.ToList();
            foreach (var roundMatchup in roundMatchups)
            {
                if (roundMatchup.PlayerOne == player)
                {
                    opponents.Add(roundMatchup.PlayerTwo);
                } else if (roundMatchup.PlayerTwo == player)
                {
                    opponents.Add(roundMatchup.PlayerOne);
                }
            }
            return opponents;
        }

        //Returns a table to be assigned to a matchup. Also keeps a record of allocated tables for round. 
        public int AllocateTable(List<int> tables, List<int> allocated)
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
                
                
                for (int i = 1; i <= GetnoOfTables(); i++)
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
                int i = r.Next(1, GetnoOfTables());
                allocated.Add(i);
                return i;
            }

            //At the moment this is just to return a number when there are no more possible combinations for players and tables
            return 999;
            
        }
        public int GetnoOfTables()
        {
            
            // var TableNumber = object.getElementById("noOfTables");

            return 5;
        }
        //returns a list of available tables that the player has not played on
        public List<int> GetTables(Player currentPlayer)
        {

            // List<int> tables = _context.RoundMatchups.Where(a => a.PlayerOne == currentPlayer || a.PlayerOne.CurrentOpponent == currentPlayer).Select(a => a.Table).ToList();
            List<int> tables = new List<int>();
            List <RoundMatchups> roundMatchups = _context.RoundMatchups.ToList();
            int currentRound = 1;
            if (_context.RoundMatchups.LastOrDefault() != null)
            {
                currentRound = _context.RoundMatchups.Last().RoundNo+1;
            }
            
                //adds all tables to list
                for (int j = 1; j <= GetnoOfTables(); j++)
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
                    if (roundMatchup.PlayerOne == currentPlayer || roundMatchup.PlayerTwo == currentPlayer || currentPlayer.CurrentOpponent ==roundMatchup.PlayerOne || currentPlayer.CurrentOpponent == roundMatchup.PlayerTwo)
                    {
                        tables.Remove(roundMatchup.Table);
                    }
                }
                //*/
               
            }
            


            //Where(r => r.roundNo == currentRound);
            return tables;
        }

        public async Task<int> GetCurrentRound()
        {
            int currentRound = 1;
            RoundMatchups lastRound = _context.RoundMatchups.LastOrDefault();
            if ( lastRound != null)
            {
                currentRound = lastRound.RoundNo;
            }
            return currentRound;
        }

        public ActionResult ValidateAllMatchups()
        {
            TempData["DuplicateOpponents"] = ValidateMatchupOpponents();
            return View("AllRounds", _context.RoundMatchups.ToList());
        }

        public Dictionary<Player, List<Player>> ValidateMatchupOpponents()
        {
            List<Player> players = _context.Players.ToList();
            Dictionary<Player, List<Player>> duplicateOpponents = new Dictionary<Player, List<Player>>();
            foreach (Player player in players)
            {
                List<Player> opponents = getAllOpponents(player);
                var duplicates = opponents
                    .GroupBy(i => i)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                duplicateOpponents[player] = duplicates;
            }

            return duplicateOpponents;
        }

        public void GenerateRoundMatchupsAlgorithm()
        {
            List<Player> players = _context.Players.OrderByDescending(p => p.BattleScore).ToList();
            List<int> AllocatedTables = new List<int>(GetnoOfTables());
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
                            var opponents = getAllOpponents(players[i]);
                            var hasPlayed = false;
                            foreach (Player opponent in opponents)
                            {
                                if (players[s] == opponent) { hasPlayed = true; }
                            }
                            Debug.WriteLine(hasPlayed);
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

            int currentRound = 1;
            if (_context.RoundMatchups.LastOrDefault() != null)
            {
                currentRound = _context.RoundMatchups.Last().RoundNo + 1;
            }
            foreach (Player player in players)
            {
                if (players.IndexOf(player) < players.IndexOf(player.CurrentOpponent))
                {
                    RoundMatchups roundMatchup = new RoundMatchups
                    {
                        RoundNo = currentRound,
                        PlayerOne = player,
                        PlayerTwo = player.CurrentOpponent
                    };
                    
                    //allocates table for matchup
                    roundMatchup.Table = AllocateTable(GetTables(player), AllocatedTables);
                    
                    _context.Add(roundMatchup);
                }
            }
            foreach (Player player in players)
            {
                player.CurrentOpponent = null;
                _context.Update(player);
            }
            if (ModelState.IsValid)
            {
                 _context.SaveChanges();
            }

        }

        public void SetAllPlayerBattleScores()
        {
            Dictionary<Player, int> playerBattleScores = new Dictionary<Player, int>();
            List<Player> players = _context.Players.ToList();
            List<RoundMatchups> roundMatchups = _context.RoundMatchups.ToList();
            foreach (Player player in players)
            {
                playerBattleScores.Add(player, 0);
            }
            foreach (RoundMatchups roundMatchup in roundMatchups)
            {
                playerBattleScores[roundMatchup.PlayerOne] += roundMatchup.PlayerOneBattleScore;
                playerBattleScores[roundMatchup.PlayerTwo] += roundMatchup.PlayerTwoBattleScore;
            }
            foreach (KeyValuePair<Player, int> playerBattleScore in playerBattleScores)
            {
                playerBattleScore.Key.BattleScore = playerBattleScore.Value;
                _context.Update(playerBattleScore.Key);
            }
            if (ModelState.IsValid)
            {
                _context.SaveChanges();
            }
        }
    } 
}
