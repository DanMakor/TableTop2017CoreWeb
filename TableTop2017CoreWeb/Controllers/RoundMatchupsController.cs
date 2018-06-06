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
            _context.SaveChanges();
            foreach (Player player in _context.Players.ToList())
            {
                player.BattleScore = 0;
                player.SportsmanshipScore = 0;
                player.ArmyScore = 0;
                _context.SaveChanges();
            }
            //PlayerActions.SetAllPlayerScores(_context);
        }

        public ActionResult DisplayNextRound()
        {
            List<Player> activePlayers = _context.Players.Where(p => p.Active == true).Where(p => p.Bye == false).ToList();
            if ((activePlayers.Count() % 2) != 0)
            {
                TempData["Errors"] = "You are attempting to generate a round with an odd number of players, choose the player that should have a bye and retry";
                return RedirectToAction(nameof(Index), "Players");
            }
            if (RoundMatchupActions.GenerateNextRound(_context) == false)
            {
                TempData["Errors"] = "There are no unique matchups left, manual selection will be required (Matchups are generated based on most evenly skilled players according to BattleScore, with no regard for previous opponents)";
            }
            return RedirectToAction(nameof(Index), "Admin");
        }

        public ActionResult DisplayNextPairRound()
        {
            List<Player> activePlayers = _context.Players.Where(p => p.Active == true).Where(p => p.Bye == false).ToList();
            if ((activePlayers.Count() % 4) != 0)
            {
                TempData["Errors"] = "You are attempting to generate a round with a number of players indivisible by four, choose the players that should have a bye and retry";
                return RedirectToAction(nameof(Index), "Players");
            }
            if (RoundMatchupActions.GenerateNextPairRound(_context) == false)
            {
                TempData["Errors"] = "There are no unique matchups left, manual selection will be required (Random Matchups have been allocated where opponents from last round are teammates)";
            }
            return RedirectToAction(nameof(Index), "Admin");
        }

        // GET: RoundMatchups
        public async Task<IActionResult> Index()
        {
            int lastRoundNo = RoundMatchupActions.GetLastRoundNo(_context);
            //Get all rounds as list from database and then filter using the where clause (potentially inefficient but no time to sort out lazy loading issue)
            var roundMatchups = _context.RoundMatchups.Where(r => !(r is PairRoundMatchup)).Where(r => r.RoundNo == lastRoundNo).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList();
            var pairRoundMatchups = _context.PairRoundMatchups.Where(r => r.RoundNo == lastRoundNo).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).ToList();
            return View(roundMatchups.Union(pairRoundMatchups));
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
                        Debug.WriteLine("THIS IS THE ALLOCATED[i]----))*  \n" + allocated[i]);
                        isAvailable = false;
                    }
                }
                if (isAvailable == true)
                {
                    allocated.Add(tableNo);
                    return tableNo;
                }
              
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
            List <RoundMatchup> roundMatchups = _context.RoundMatchups.ToList();
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
    } 
}
