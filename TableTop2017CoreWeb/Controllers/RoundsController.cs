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

        public async Task<IActionResult> Index()
        {
            return View(await _context.RoundsModel.ToListAsync());
        }

       /* public async Task<int> GetLastRound(TournamentDbContext _context)
        {
            int currentRound = 1;
            Rounds lastRound = await _context.Rounds.LastOrDefaultAsync();
            if (lastRound != null)
            {
                currentRound = lastRound.RoundNo;
            }
            return currentRound;
        }
        */
        private bool RoundsModelExists(int id)
        {
            return _context.RoundsModel.Any(e => e.Id == id);
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
                        return RedirectToAction("DisplayNextRound", "Admin");
                    }
                    break;
                case "GoToDisplayNextPairRound":
                    // call another action to perform the cancellation
                    if (ModelState.IsValid)
                    {
                        _context.Add(Round);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("DisplayNextPairRound", "Admin");
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
            if (_context.RoundsModel.Count() == 0)
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
                return RedirectToAction("DisplayNextRound", "Admin");
            }

            return View(Round);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoToDisplayNextPairRound([Bind("Id,NoTableTops,NoTableTops,Players,RoundMatchups")] RoundsModel Round)
        {
            if (_context.RoundsModel.Count() == 0)
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
                return RedirectToAction("DisplayNextPairRound", "Admin");
            }

            return View(Round);
        }

     
        public async Task<IActionResult> PlayersDisplay()
        {

            var display = new RoundsModel();

            List<Player> players = await _context.Players.OrderByDescending(p => p.BattleScore).ToListAsync();
            if (_context.RoundsModel.Count() > 0)
                display.NoTableTops = _context.RoundsModel.Last().NoTableTops;
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