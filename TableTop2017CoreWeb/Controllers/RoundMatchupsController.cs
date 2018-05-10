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
using TableTop2017CoreWeb.Helpers;
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
            List<RoundMatchup> roundmatchups = _context.RoundMatchups.ToList();
            foreach(RoundMatchup roundmatchup in roundmatchups)
            {
                _context.Remove(roundmatchup);
            }
            List<RoundsModel> rounds = _context.RoundsModel.ToList();
            foreach (RoundsModel round in rounds)
            {
                _context.Remove(round);
            }

            _context.SaveChanges();
            SetPlayerBattleScores();
        }

        // GET: RoundMatchups
        public async Task<IActionResult> Index()
        {
            int currentRound = RoundMatchupActions.GetLastRoundNo(_context);
            List<RoundMatchup> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();
            return View(roundMatchups);
        }

        // GET: AdminMatchups
        public async Task<IActionResult> Admin(int? roundNumber)
        {
            int? currentRound;
            string selectedRound;
            List<RoundMatchup> roundMatchups = new List<RoundMatchup>();
            var pairRoundMatchups = new List<PairRoundMatchup>();
            if (roundNumber == 0)
            {
                roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();
                selectedRound = "all";
            }
            else
            {
                if (roundNumber == null)
                {
                    currentRound = RoundMatchupActions.GetLastRoundNo(_context);
                }
                else
                {
                    currentRound = roundNumber;
                }
                roundMatchups = await _context.RoundMatchups.Where(r => !(r is PairRoundMatchup)).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).ToListAsync();
                pairRoundMatchups = await _context.PairRoundMatchup.Where(r => r.RoundNo == currentRound).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).ToListAsync();
                selectedRound = currentRound.ToString();
            }
            AdminViewModel avm = new AdminViewModel
            {
                RoundMatchup = pairRoundMatchups,
                NoOfRounds = _context.RoundMatchups.Select(r => r.RoundNo).ToArray(),
                CurrentRound = selectedRound
            };
            if ( avm.RoundMatchup.Count() == 0)
            {
                avm.RoundMatchup = roundMatchups;
            }
            TempData["DuplicatePlayers"] = null;
            TempData["OverallocatedPlayers"] = null;
            TempData["UnallocatedPlayers"] = null;

            return View(avm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin([Bind("CurrentRound")] AdminViewModel avm)
        {
            int currentRound;


            if (avm.CurrentRound == "all")
            {
                currentRound = 0;
            }
            else
            {
                currentRound = int.Parse(avm.CurrentRound);
            }
            return RedirectToAction("Admin", "RoundMatchups", new { roundNumber = currentRound });
        }

        // GET: RoundMatchups Results
        public async Task<IActionResult> Results()
        {
            int currentRound = RoundMatchupActions.GetLastRoundNo(_context);
            List<RoundMatchup> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();
            return View(roundMatchups);
        }

        // GET: Allrounds
        public async Task<IActionResult> AllRounds()
        {
            List<RoundMatchup> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToListAsync();
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
            SetPlayerBattleScores();
            return RedirectToAction("Index", "Players");
        }
        //Update the Player Battlescores before redirecting to the player page
        public ActionResult UpdateBattleScoresContinue()
        {
            SetPlayerBattleScores();
            if (_context.RoundsModel.LastOrDefault() == null)
            {
                return RedirectToAction("PlayersDisplay", "Rounds");
            }
            /*RoundsModel newRound = new RoundsModel
            {
                //RoundNo= _context.RoundsModel.LastOrDefault().RoundNo +1,
                NoTableTops = _context.RoundsModel.LastOrDefault().NoTableTops,
         
            };
            if (ModelState.IsValid)
            {
                _context.Add(newRound);
                await _context.SaveChangesAsync();
                return RedirectToAction("DisplayNextRound", "Admin");
            }
            */

            return RedirectToAction("DisplayNextRound", "Admin");
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
        public async Task<IActionResult> ResultsEdit(int id, [Bind("Id,RoundNo,PlayerOneId,PlayerTwoId,PlayerOneBattleScore,PlayerTwoBattleScore,PlayerOneSportsmanshipScore,PlayerTwoSportsmanshipScore,Table")] RoundMatchup roundMatchup)
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
                    if (!RoundMatchupActions.RoundMatchupsExists(roundMatchup.Id, _context))
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

    public void SetPlayerBattleScores()
    {
        Dictionary<Player, int> playerBattleScores = PlayerActions.GetAllPlayerBattleScores(_context);
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


        //Generate the RoundMatchups 
        public ActionResult GenerateRoundMatchups()
        {
            RoundMatchupActions.GenerateNextRound(_context);
            return RedirectToAction("Index", "Admin");
        }
        

        // GET: RoundMatchups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).SingleOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoundNo,PlayerOneBattleScore,PlayerTwoBattleScore,PlayerOneSportsmanshipPoints,PlayerTwoSportsmanshipPoints,Table")] RoundMatchup roundMatchups)
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
                    if (!RoundMatchupActions.RoundMatchupsExists(roundMatchups.Id, _context))
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
            if (_context.RoundsModel.Count() > 0)
            {
                RoundsModel NoOfTa = _context.RoundsModel.Last();
                return NoOfTa.NoTableTops;
            }


            return 12;
        }
        //returns a list of available tables that the player has not played on
        public List<int> GetTables(Player currentPlayer)
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

        //public ActionResult ValidateAllMatchups()
        //{
        //    TempData["DuplicateOpponents"] = ValidateMatchupOpponents();
        //    return View("AllRounds", _context.RoundMatchups.ToList());
        //}

    } 
}
