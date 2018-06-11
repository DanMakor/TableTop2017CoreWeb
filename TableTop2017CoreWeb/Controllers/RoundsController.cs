using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {

            return View(await _context.Rounds.ToListAsync());
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
            return RedirectToAction("Edit","Players", player);
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

            return RedirectToAction("Delete", "Players", player);
        }

        private bool RoundsModelExists(int id)
        {
            return _context.Rounds.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoToAction(string submitButton, [Bind("Id,RoundNo,NoTableTops,Players,RoundMatchups")] RoundsModel Round)
        {
            switch (submitButton)
            {
                case "GoToDisplayNextRound":
                    // delegate sending to another controller action
                    if (ModelState.IsValid)
                    {
                        _context.Add(Round);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("DisplayNextRound", "RoundMatchups");
                    }
                    break;
                case "GoToDisplayNextPairRound":
                    // call another action to perform the cancellation
                    if (ModelState.IsValid)
                    {
                        _context.Add(Round);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("DisplayNextPairRound", "RoundMatchups");
                    }
                    break;
                default:
                    // If they've submitted the form without a submitButton, 
                    // just return the view again.
                    return (View(Round));
            }
            return (View(Round));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoToDisplayNextRound([Bind("Id,RoundNo,NoTableTops,Players,RoundMatchups")] RoundsModel Round)
        {
            if (_context.Rounds.Count() == 0)
            {
               // Round.RoundNo = 1;
            }
            else
            {
               // Round.RoundNo = _context.RoundsModel.Last().RoundNo + 1;
            }

            if (ModelState.IsValid)
            {
                _context.Add(Round);
                await _context.SaveChangesAsync();
                return RedirectToAction("DisplayNextRound", "RoundMatchups");
            }

            return View(Round);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoToDisplayNextPairRound([Bind("Id,NoTableTops,NoTableTops,Players,RoundMatchups")] RoundsModel Round)
        {
            if (_context.Rounds.Count() == 0)
            {
               // Round.RoundNo = 1;
            }
            else
            {
                //Round.RoundNo = _context.RoundsModel.Last().RoundNo + 1;
            }

            if (ModelState.IsValid)
            {
                _context.Add(Round);
                await _context.SaveChangesAsync();
                return RedirectToAction("DisplayNextPairRound", "RoundMatchups");
            }

            return View(Round);
        }

     
        public async Task<IActionResult> PlayersDisplay()
        {
            List<Player> players = await _context.Players.ToListAsync();
            ViewData["Errors"] = TempData["Errors"];
            ViewData["ExportStatus"] = TempData["ExportStatus"];
            Tournament tournament = _context.Tournaments.First();
            foreach (Player player in players)
            {
                player.WeightedScore = ((int)(player.BattleScore * tournament.BattleScoreRatio) + (int)(player.SportsmanshipScore * tournament.SportsmanshipScoreRatio) + (int)(player.ArmyScore * tournament.ArmyScoreRatio));
            }
            players = players.OrderByDescending(p => p.WeightedScore).ToList();
            //  return View(players);
            var display = new RoundsModel();
            
            if (_context.Rounds.Count() > 0)
                display.NoTableTops = _context.Rounds.Last().NoTableTops;
            display.Players = players;
            return View(display);
        }



        //gets all players from players model
        public List<Player> GetPlayers()
        {
            List<Player> players = _context.Players.OrderByDescending(p => p.BattleScore).ToList();
            return players;

        }
        //Gets all active players from Players Model
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

        public List<RoundMatchup> GetRoundMatchups()
        {
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.ToList();
            var currentRound = 0;
            if (_context.Rounds.Last().RoundNo > 0)
                currentRound = _context.Rounds.Last().RoundNo++;
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


        public async Task<int> GetLastRound(TournamentDbContext _context)
        {
            int currentRound = 1;
            RoundsModel lastRound = await _context.Rounds.LastOrDefaultAsync();
            if (lastRound != null)
            {
                currentRound = lastRound.RoundNo;
            }
            return currentRound;
        }
    }
}