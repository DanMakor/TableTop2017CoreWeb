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
                currentRound = _context.RoundMatchups.Last().roundNo;
            }

            //Where(r => r.roundNo == currentRound);
            List<RoundMatchups> roundMatchups = await _context.RoundMatchups.Include(r => r.player).Where(r => r.roundNo == currentRound).OrderByDescending(r => r.player.totalBattleScore).ToListAsync();
            return View(roundMatchups);
        }

        //Generate the RoundMatchups 

        public async Task<IActionResult> GenerateRoundMatchups()
        {
            List<Player> players = _context.Players.OrderByDescending(p => p.totalBattleScore).ToList();
            int secondaryIndex = 0;
            int i = 0;
            while (i < players.Count)
            {
                //Skip this player if they are already allocated a player
                if (players[i].currentOpponent == null)
                {
                    if (secondaryIndex == 0) { secondaryIndex = i + 1; }
                    int s = 0;

                    for (s = secondaryIndex; s < players.Count; s++)
                    {
                        if (players[s].currentOpponent == null)
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
                                players[i].currentOpponent = players[s];
                                players[s].currentOpponent = players[i];
                                secondaryIndex = 0;
                                break;
                            }
                            //If there was no one to allocate, de allocate previous player
                            if (s == (players.Count - 1) && (players[i].currentOpponent == null))
                            {
                                if (i - 1 >= 0)
                                {
                                    //Ensure that the player one index above the current player has an opponent
                                    //Need to make it so that we deallocate the lowest of the higher player in a matchup rather than just the next player 
                                    int previousOpponent = i - 1;
                                    while (players[previousOpponent].currentOpponent == null)
                                    {
                                        previousOpponent += 1;
                                    }
                                    //If the previous player has a lower score than the previous players opponent, set i to the previous players opponent
                                    //and s to the previous player + 1
                                    if (players.IndexOf(players[previousOpponent].currentOpponent) < players.IndexOf(players[previousOpponent]))
                                    {
                                        secondaryIndex = previousOpponent + 1;
                                        i = players.IndexOf(players[previousOpponent].currentOpponent) - 1;
                                    }
                                    //If the previous player has a higher score than the previous players opponent, set i to the previous player and s to 
                                    //the previous players opponent
                                    else
                                    {
                                        secondaryIndex = players.IndexOf(players[previousOpponent].currentOpponent) + 1;
                                        i = previousOpponent - 1;
                                    }
                                    //Remove the opponents of the previous player and the previous players opponent
                                    players[i + 1].currentOpponent.currentOpponent = null;
                                    players[i + 1].currentOpponent = null;
                                    
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
                currentRound = _context.RoundMatchups.Last().roundNo + 1;
            }
            foreach (Player player in players)
            {
                RoundMatchups roundMatchup = new RoundMatchups
                {
                    roundNo = currentRound,
                    player = player,
                    opponent = player.currentOpponent
                };
                player.currentOpponent = null;
                _context.Add(roundMatchup);
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

            var roundMatchups = await _context.RoundMatchups.Include(r => r.player)
                .SingleOrDefaultAsync(m => m.id == id);
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

            var roundMatchups = await _context.RoundMatchups.Include(r => r.player).SingleOrDefaultAsync(m => m.id == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("id,roundNo,battleScore,sportsmanshipPoints,table")] RoundMatchups roundMatchups)
        {
            if (id != roundMatchups.id)
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
                    if (!RoundMatchupsExists(roundMatchups.id))
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

            var roundMatchups = await _context.RoundMatchups.Include(r => r.player)
                .SingleOrDefaultAsync(m => m.id == id);
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
            var roundMatchups = await _context.RoundMatchups.SingleOrDefaultAsync(m => m.id == id);
            _context.RoundMatchups.Remove(roundMatchups);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoundMatchupsExists(int id)
        {
            return _context.RoundMatchups.Any(e => e.id == id);
        }
        public List<Player> getAllOpponents(Player player)
        {
            List<Player> opponents = new List<Player>();
            List<RoundMatchups> roundMatchups = _context.RoundMatchups.ToList();
            foreach (var roundMatchup in roundMatchups)
            {
                if (roundMatchup.player == player)
                {
                    opponents.Add(roundMatchup.opponent);
                }
            }
            return opponents;
        }

    }
}
