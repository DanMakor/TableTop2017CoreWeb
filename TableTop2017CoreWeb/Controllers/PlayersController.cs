using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TableTop2017CoreWeb.Data;
using TableTop2017CoreWeb.Models;

namespace TableTop2017CoreWeb.Controllers
{
    public class PlayersController : Controller
    {
        private readonly TournamentDbContext _context;

        public PlayersController(TournamentDbContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            List<Player> players = await _context.Players.OrderByDescending(p => p.BattleScore).ToListAsync();
            return RedirectToAction("PlayersDisplay", "Rounds", players);
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .SingleOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            Player player = new Player();
            return View(player);
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,BattleScore,SportsmanshipScore,Army,Active,EmailAddress,Notes,Paid")] Player player)
        {
            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players.SingleOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BattleScore,SportsmanshipScore,Army,Active,EmailAddress,Notes,Paid")] Player player)
        {
            if (id != player.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id))
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
            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .SingleOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.SingleOrDefaultAsync(m => m.Id == id);
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.Id == id);
        }

        public int GetPlayerBattleScore(int? id)
        {
            if (id == null)
            {
                NotFound();
            }
            int[] posOneScores = _context.RoundMatchups.Where(p => p.PlayerOne.Id == id).Select(p => p.PlayerOneBattleScore).ToArray();
            int[] posTwoScores = _context.RoundMatchups.Where(p => p.PlayerTwo.Id == id).Select(p => p.PlayerTwoBattleScore).ToArray();
            return posOneScores.Sum() + posTwoScores.Sum();
        }

        public void SetAllPlayerBattleScores()
        {
            Dictionary<Player, int> playerBattleScores = new Dictionary<Player, int>();
            List<Player> players = _context.Players.ToList();
            foreach (Player player in players)
            {
                playerBattleScores.Add(player, 0);
            }
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.ToList();
            foreach (RoundMatchup roundMatchup in roundMatchups)
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
