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
    public class TournamentsController : Controller
    {
        private readonly TournamentDbContext _context;

        public TournamentsController(TournamentDbContext context)
        {
            _context = context;
        }

        //Single tournament is seeded into the database upon application startup
        //The logic for this can be found in Initialize.cs and Program.cs

        // GET: Tournaments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.SingleOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BattleScoreRatio,SportsmanshipScoreRatio,ArmyScoreRatio")] Tournament tournament)
        {
            if (id != tournament.Id)
            {
                return NotFound();
            }

            if (tournament.BattleScoreRatio + tournament.SportsmanshipScoreRatio + tournament.ArmyScoreRatio != 1)
            {
                TempData["Errors"] = "The score ratios must add up to a total of 1";
                return View(tournament);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Players");
            }
            return View(tournament);
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.Id == id);
        }
    }
}
