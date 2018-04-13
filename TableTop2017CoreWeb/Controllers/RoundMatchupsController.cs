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
        }

        // GET: RoundMatchups
        public async Task<IActionResult> Index()
        {
            int currentRound = 1;
            if (_context.RoundMatchups.LastOrDefault() != null)
            {
                currentRound = _context.RoundMatchups.Last().RoundNo;
            }

            //Where(r => r.roundNo == currentRound);
            List<RoundMatchups> roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Where(r => r.RoundNo == currentRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToListAsync();
            return View(roundMatchups);
        }

        //Generate the RoundMatchups 

        public async Task<IActionResult> GenerateRoundMatchups()
        {
            List<Player> players = _context.Players.OrderByDescending(p => p.BattleScore).ToList();
            int secondaryIndex = 0;
            int i = 0;
            while (i < players.Count)
            {
                //Skip this player if they are already allocated a player
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
                            if (s == (players.Count - 1) && (players[i].CurrentOpponent == null))
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
                if (players.IndexOf(player) < players.IndexOf(player.CurrentOpponent)) { 
                    RoundMatchups roundMatchup = new RoundMatchups
                    {
                        RoundNo = currentRound,
                        PlayerOne = player,
                        PlayerTwo = player.CurrentOpponent
                    };
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
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // GET: RoundMatchups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoundNo,PlayerOnebattleScore,PlayerTwoBattleScore,PlayerOneSportsmanshipPoints,PlayerTwoSportsmanshipPoints,Table")] RoundMatchups roundMatchups)
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

            var roundMatchups = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo)
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
                }
            }
            return opponents;
        }

    }
}
