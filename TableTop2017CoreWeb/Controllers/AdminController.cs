using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TableTop2017CoreWeb.Models;
using TableTop2017CoreWeb.Data;
using Microsoft.EntityFrameworkCore;
using TableTop2017CoreWeb.Helpers;
using Newtonsoft.Json;

namespace TableTop2017CoreWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly TournamentDbContext _context;

        public AdminController(TournamentDbContext context)
        {
            _context = context;
        }

        // GET: AdminMatchups
        public async Task<IActionResult> Index()
        {
            int lastRoundNo = RoundMatchupActions.GetLastRoundNo(_context);

            ViewData["Errors"] = TempData["Errors"];
            ViewData["DuplicatePlayers"] = TempData["DuplicatePlayers"];
            ViewData["DuplicateOpponents"] = TempData["DuplicateOpponents"];
            ViewData["OverallocatedPlayers"] = TempData["OverallocatedPlayers"];
            ViewData["UnallocatedPlayers"] = TempData["UnallocatedPlayers"];

            return View(_context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList().Where(r => r.RoundNo == lastRoundNo).ToList());

        }

        //GET: AdminEdit
        public async Task<IActionResult> Edit(int? id)
        {
            var aevm = new AdminEditViewModel();

            if (id == null)
            {
                return NotFound();
            }

            var roundMatchup = await _context.RoundMatchups.Where(r => r.Id == id).Where(r => !(r is PairRoundMatchup)).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).SingleOrDefaultAsync();
            var pairRoundMatchup = await _context.PairRoundMatchup.Where(r => r.Id == id).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).SingleOrDefaultAsync();
            if (roundMatchup != null)
            {
                aevm.RoundMatchup = new AdminEditRoundMatchupsViewModel
                {
                    Id = roundMatchup.Id,
                    PlayerOneId = roundMatchup.PlayerOne.Id,
                };
                //Handling bye roundmatchup
                //If this roundmatchup is not a bye, set PlayerTwoId in the view model to the value of PlayerTwo.Id in the roundMatchup
                if (roundMatchup.PlayerTwo != null)
                {
                    aevm.RoundMatchup.PlayerTwoId = roundMatchup.PlayerTwo.Id;
                }
                //If this roundmatchup is a bye, set PlayerTwoId in the view model to 0.5 (not 0 because that could be an Id)
                else
                {
                    aevm.RoundMatchup.PlayerTwoId = 0.5;
                }
            } else if (pairRoundMatchup != null)
            {
                aevm.RoundMatchup = new AdminEditRoundMatchupsViewModel
                {
                    Id = pairRoundMatchup.Id,
                    PlayerOneId = pairRoundMatchup.PlayerOne.Id,
                };
                //Handling bye roundmatchup
                //If this roundmatchup is not a bye, set PlayerTwoId, PlayerFourId in the view model to the value of PlayerTwo.Id in the roundMatchup

                if (pairRoundMatchup.PlayerTwo != null || pairRoundMatchup.PlayerFour != null)
                {
                    aevm.RoundMatchup.PlayerTwoId = pairRoundMatchup.PlayerTwo.Id;
                    aevm.RoundMatchup.PlayerThreeId = pairRoundMatchup.PlayerThree.Id;
                    aevm.RoundMatchup.PlayerFourId = pairRoundMatchup.PlayerFour.Id;
                }
                //If this roundmatchup is a bye, set PlayerTwoId, PlayerFourId in the view model to 0.5 (not 0 because that could be an Id)
                else
                {
                    aevm.RoundMatchup.PlayerTwoId = 0.5;
                    aevm.RoundMatchup.PlayerThreeId = 0.5;
                    aevm.RoundMatchup.PlayerFourId = 0.5;
                }
            }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlayerOneId,PlayerTwoId,PlayerThreeId,PlayerFourId,CurrentRound")] AdminEditRoundMatchupsViewModel roundMatchup)
        {
            if (id != roundMatchup.Id)
            {
                return NotFound();
            }
            PairRoundMatchup updatedPairRoundMatchup = null;
            RoundMatchup updatedRoundMatchup = null;
            if (_context.RoundMatchups.Find(id) is PairRoundMatchup)
            {
                updatedPairRoundMatchup = _context.RoundMatchups.Find(id) as PairRoundMatchup;
                updatedPairRoundMatchup.PlayerOne = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerOneId);
                updatedPairRoundMatchup.PlayerTwo = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerTwoId);
                updatedPairRoundMatchup.PlayerThree = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerThreeId);
                updatedPairRoundMatchup.PlayerFour = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerFourId);
            } else
            {
                updatedRoundMatchup = _context.RoundMatchups.Find(id);
                updatedRoundMatchup.PlayerOne = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerOneId);
                updatedRoundMatchup.PlayerTwo = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerTwoId);
            }
                
            

            if (ModelState.IsValid)
            {
                try
                {
                    if (updatedPairRoundMatchup != null)
                    {
                        _context.Update(updatedPairRoundMatchup);
                    }
                    else
                    {
                        _context.Update(updatedRoundMatchup);
                    }

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
                return RedirectToAction(nameof(Index));
            }
            return View(roundMatchup);
        }

        public ActionResult ValidateRoundMatchups()
        {
            List<List<string>> errors = RoundMatchupActions.GetRoundMatchupErrors(_context);
            if (errors[0].Count == 0 && errors[1].Count == 0 && errors[2].Count == 0 && errors[3].Count == 0)
            {
                TempData["Status"] = "You're good to go!";
            }
            TempData["DuplicatePlayers"] = string.Join(" | ", errors[0]);
            TempData["DuplicateOpponents"] = string.Join(" | ", errors[1]);
            TempData["OverallocatedPlayers"] = string.Join(" | ", errors[2]);
            TempData["UnallocatedPlayers"] = string.Join(" | ", errors[3]);
            return RedirectToAction(nameof(Index));
        }

        //Reset the matchups 
        public ActionResult ResetRoundMatchups()
        {
            int roundNo = RoundMatchupActions.GetLastRoundNo(_context);
            Boolean pairRoundMatchup = false;
            foreach (RoundMatchup roundMatchup in _context.RoundMatchups.Where(r => r.RoundNo == roundNo).ToList())
            {
                if (roundMatchup is PairRoundMatchup) { pairRoundMatchup = true; }
                _context.Remove(roundMatchup);
            }
             _context.SaveChanges();
            if (pairRoundMatchup == true)
            {
                RoundMatchupActions.GenerateNextPairRound( _context);
            }
            else
            {
                RoundMatchupActions.GenerateNextRound(_context);
            }
            return RedirectToAction(nameof(Index), "Admin");
        }

        /**
         * 
         * AllRounds
         * 
         **/

        //GET: All Rounds
        public ActionResult AllRounds()
        {
            ViewData["Status"] = TempData["Status"];
            ViewData["DuplicatePlayers"] = TempData["DuplicatePlayers"];
            ViewData["DuplicateOpponents"] = TempData["DuplicateOpponents"];
            ViewData["OverallocatedPlayers"] = TempData["OverallocatedPlayers"];
            ViewData["UnallocatedPlayers"] = TempData["UnallocatedPlayers"];
            return View(_context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList());
        }

        //GET: AllRoundsEdit (Edit one round)
        public async Task<IActionResult> AllRoundsEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var roundMatchup = await _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).SingleOrDefaultAsync(m => m.Id == id);
            var players = await _context.Players.ToListAsync();

            if (roundMatchup is PairRoundMatchup)
            {
                var pairRoundMatchup = roundMatchup as PairRoundMatchup;
                var prmevm = new PairRoundMatchupEditViewModel()
                {
                    Id = pairRoundMatchup.Id,
                    RoundNo = pairRoundMatchup.RoundNo,
                    PlayerOneId = pairRoundMatchup.PlayerOne.Id,
                    PlayerOneBattleScore = pairRoundMatchup.PlayerOneBattleScore,
                    PlayerOneSportsmanshipScore = pairRoundMatchup.PlayerOneSportsmanshipScore,
                    Table = pairRoundMatchup.Table,
                    Players = players
                };
                if (pairRoundMatchup.PlayerTwo != null && pairRoundMatchup.PlayerFour != null)
                {
                    prmevm.PlayerTwoId = pairRoundMatchup.PlayerTwo.Id;
                    prmevm.PlayerTwoBattleScore = pairRoundMatchup.PlayerTwoBattleScore;
                    prmevm.PlayerTwoSportsmanshipScore = pairRoundMatchup.PlayerTwoSportsmanshipScore;
                    prmevm.PlayerThreeId = pairRoundMatchup.PlayerThree.Id;
                    prmevm.PlayerThreeBattleScore = pairRoundMatchup.PlayerThreeBattleScore;
                    prmevm.PlayerThreeSportsmanshipScore = pairRoundMatchup.PlayerThreeSportsmanshipScore;
                    prmevm.PlayerFourId = pairRoundMatchup.PlayerFour.Id;
                    prmevm.PlayerFourBattleScore = pairRoundMatchup.PlayerFourBattleScore;
                    prmevm.PlayerFourSportsmanshipScore = pairRoundMatchup.PlayerFourSportsmanshipScore;
                }
                else
                {
                    prmevm.PlayerTwoId = 0.5;
                    prmevm.PlayerThreeId = 0.5;
                    prmevm.PlayerFourId = 0.5;
                }
                return View("PairRoundMatchupEdit", prmevm);
            }
            else
            {
                var arevm = new RoundMatchupEditViewModel()
                {
                    Id = roundMatchup.Id,
                    RoundNo = roundMatchup.RoundNo,
                    PlayerOneId = roundMatchup.PlayerOne.Id,
                    PlayerOneBattleScore = roundMatchup.PlayerOneBattleScore,
                    PlayerOneSportsmanshipScore = roundMatchup.PlayerOneSportsmanshipScore,
                    Table = roundMatchup.Table,
                    Players = players
                };
                if (roundMatchup.PlayerTwo != null)
                {
                    arevm.PlayerTwoId = roundMatchup.PlayerTwo.Id;
                    arevm.PlayerTwoBattleScore = roundMatchup.PlayerTwoBattleScore;
                    arevm.PlayerTwoSportsmanshipScore = roundMatchup.PlayerTwoSportsmanshipScore;
                }
                else
                {
                    arevm.PlayerTwoId = 0.5;
                }
                return View("RoundMatchupEdit", arevm);
            }
        }

        //POST: RoundMatchupEdit Update record for a standard roundMatchup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoundMatchupEdit(int id, [Bind("Id, RoundNo, PlayerOneId, PlayerTwoId, PlayerOneBattleScore, PlayerTwoBattleScore, PlayerOneSportsmanshipScore, PlayerTwoSportsmanshipScore, Table")] RoundMatchupEditViewModel roundMatchupvm)
        {
            if (id != roundMatchupvm.Id)
            {
                return NotFound();
            }

            var playerOne = await _context.Players.SingleOrDefaultAsync(p => p.Id == roundMatchupvm.PlayerOneId);
            var playerTwo = await _context.Players.SingleOrDefaultAsync(p => p.Id == roundMatchupvm.PlayerTwoId);
            var roundMatchup = new RoundMatchup()
            {
                Id = roundMatchupvm.Id,
                RoundNo = roundMatchupvm.RoundNo,
                PlayerOne = playerOne,
                PlayerOneBattleScore = roundMatchupvm.PlayerOneBattleScore,
                PlayerOneSportsmanshipScore = roundMatchupvm.PlayerOneSportsmanshipScore,
                PlayerTwo = playerTwo,
                PlayerTwoBattleScore = roundMatchupvm.PlayerTwoBattleScore,
                PlayerTwoSportsmanshipScore = roundMatchupvm.PlayerTwoSportsmanshipScore,
                Table = roundMatchupvm.Table
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roundMatchup);
                    await _context.SaveChangesAsync();
                    PlayerActions.SetPlayerScores((int)roundMatchupvm.PlayerOneId, _context);
                    PlayerActions.SetPlayerScores((int)roundMatchupvm.PlayerTwoId, _context);
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
                return RedirectToAction(nameof(AllRounds));
            }
            return View(roundMatchupvm);
        }

        //POST: PairRoundMatchupEdit Update record for a pair roundMatchup
        //Can look at setting up an Editor Template for RoundMatchups that deals with the Player Complex Object in a RoundMatchup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PairRoundMatchupEdit(int id, [Bind("Id,RoundNo,PlayerOneId,PlayerTwoId,PlayerThreeId,PlayerFourId,PlayerOneBattleScore,PlayerTwoBattleScore,PlayerThreeBattleScore,PlayerFourBattleScore,PlayerOneSportsmanshipScore,PlayerTwoSportsmanshipScore,PlayerThreeSportsmanshipScore,PlayerFourSportsmanshipScore,Table")] PairRoundMatchupEditViewModel pairRoundMatchupvm)
        {
            if (id != pairRoundMatchupvm.Id)
            {
                return NotFound();
            }

            var playerOne = await _context.Players.SingleOrDefaultAsync(p => p.Id == pairRoundMatchupvm.PlayerOneId);
            var playerTwo = await _context.Players.SingleOrDefaultAsync(p => p.Id == pairRoundMatchupvm.PlayerTwoId);
            var playerThree = await _context.Players.SingleOrDefaultAsync(p => p.Id == pairRoundMatchupvm.PlayerThreeId);
            var playerFour = await _context.Players.SingleOrDefaultAsync(p => p.Id == pairRoundMatchupvm.PlayerFourId);
            var roundMatchup = new PairRoundMatchup()
            {
                Id = pairRoundMatchupvm.Id,
                RoundNo = pairRoundMatchupvm.RoundNo,
                PlayerOne = playerOne,
                PlayerOneBattleScore = pairRoundMatchupvm.PlayerOneBattleScore,
                PlayerOneSportsmanshipScore = pairRoundMatchupvm.PlayerOneSportsmanshipScore,
                PlayerTwo = playerTwo,
                PlayerTwoBattleScore = pairRoundMatchupvm.PlayerTwoBattleScore,
                PlayerTwoSportsmanshipScore = pairRoundMatchupvm.PlayerTwoSportsmanshipScore,
                PlayerThree = playerThree,
                PlayerThreeBattleScore = pairRoundMatchupvm.PlayerThreeBattleScore,
                PlayerThreeSportsmanshipScore = pairRoundMatchupvm.PlayerThreeSportsmanshipScore,
                PlayerFour = playerFour,
                PlayerFourBattleScore = pairRoundMatchupvm.PlayerFourBattleScore,
                PlayerFourSportsmanshipScore = pairRoundMatchupvm.PlayerFourSportsmanshipScore,
                Table = pairRoundMatchupvm.Table
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roundMatchup);
                    await _context.SaveChangesAsync();
                    if (playerOne != null) PlayerActions.SetPlayerScores((int)pairRoundMatchupvm.PlayerOneId, _context);
                    if (playerTwo != null) PlayerActions.SetPlayerScores((int)pairRoundMatchupvm.PlayerTwoId, _context);
                    if (playerThree != null) PlayerActions.SetPlayerScores((int)pairRoundMatchupvm.PlayerThreeId, _context);
                    if (playerFour != null) PlayerActions.SetPlayerScores((int)pairRoundMatchupvm.PlayerFourId, _context);
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
                return RedirectToAction(nameof(AllRounds));
            }
            return View(pairRoundMatchupvm);
        }

        public ActionResult ValidateAllRoundMatchups()
        {
            List<List<string>> errors = RoundMatchupActions.GetRoundMatchupErrors( _context);
            if (errors[0].Count == 0 && errors[1].Count == 0 && errors[2].Count == 0 && errors[3].Count == 0)
            {
                TempData["Status"] = "You're good to go!";
            }
            TempData["DuplicatePlayers"] = string.Join(" | ", errors[0]);
            TempData["DuplicateOpponents"] = string.Join(" | ", errors[1]);
            TempData["OverallocatedPlayers"] = string.Join(" | ", errors[2]);
            TempData["UnallocatedPlayers"] = string.Join(" | ", errors[3]);
            return RedirectToAction(nameof(AllRounds));
        }
    }
}