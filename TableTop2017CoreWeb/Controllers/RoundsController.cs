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
    public class RoundsController : Controller
    {
        private readonly TournamentDbContext _context;

        public RoundsController(TournamentDbContext context)
        {
            _context = context;
        }

        // GET: Rounds
        public async Task<IActionResult> Index()
        {
            return View(await _context.RoundsModel.ToListAsync());
        }

        // GET: Rounds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundsModel = await _context.RoundsModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (roundsModel == null)
            {
                return NotFound();
            }

            return View(roundsModel);
        }

        // GET: Rounds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rounds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoundNo,NoTableTops")] RoundsModel roundsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roundsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roundsModel);
        }

        // GET: Rounds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundsModel = await _context.RoundsModel.SingleOrDefaultAsync(m => m.Id == id);
            if (roundsModel == null)
            {
                return NotFound();
            }
            return View(roundsModel);
        }

        // POST: Rounds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoundNo,NoTableTops")] RoundsModel roundsModel)
        {
            if (id != roundsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roundsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoundsModelExists(roundsModel.Id))
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
            return View(roundsModel);
        }

        // GET: Rounds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundsModel = await _context.RoundsModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (roundsModel == null)
            {
                return NotFound();
            }

            return View(roundsModel);
        }

        // POST: Rounds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roundsModel = await _context.RoundsModel.SingleOrDefaultAsync(m => m.Id == id);
            _context.RoundsModel.Remove(roundsModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoundsModelExists(int id)
        {
            return _context.RoundsModel.Any(e => e.Id == id);
        }

        public List<Player> GetActivePlayers()
        {
            List<Player> players = _context.Players.OrderByDescending(p => p.BattleScore).ToList();

            foreach (var player in players)
            {
                if (player.Active == false)
                {
                    players.Remove(player);
                }
            }
            return players;

        }

        public List<RoundMatchups> GetRoundMatchups()
        {
            List<RoundMatchups> roundMatchups = _context.RoundMatchups.ToList();
            var currentRound = 0;
            if (_context.RoundsModel.Last().RoundNo > 0)
                currentRound = _context.RoundsModel.Last().RoundNo++;
           // currentRpund = _context.RoundsModel.RoundNo.Last();
            foreach (var matchup in roundMatchups)
            {
                if (matchup.RoundNo != currentRound)
                {
                    roundMatchups.Remove(matchup);
                }
            }
            return roundMatchups;

        }


    }
}
