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
            ViewData["DuplicateOpponents"] = TempData["DuplicateOpponents"];
            ViewData["OverallocatedPlayers"] = TempData["OverallocatedPlayers"];
            ViewData["UnallocatedPlayers"] = TempData["UnallocatedPlayers"];

            var roundMatchups = _context.RoundMatchups.Where(r => !(r is PairRoundMatchup)).Where(r => r.RoundNo == lastRoundNo).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList();
            var pairRoundMatchups = _context.PairRoundMatchups.Where(r => r.RoundNo == lastRoundNo).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).ToList();
            return View(roundMatchups.Union(pairRoundMatchups));

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
            var pairRoundMatchup = await _context.PairRoundMatchups.Where(r => r.Id == id).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).SingleOrDefaultAsync();
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
                    aevm.RoundMatchup.TableNo = roundMatchup.Table;
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
                if (pairRoundMatchup.PlayerTwo != null)
                {
                    aevm.RoundMatchup.PlayerTwoId = pairRoundMatchup.PlayerTwo.Id;
                    aevm.RoundMatchup.PlayerThreeId = pairRoundMatchup.PlayerThree.Id;
                    aevm.RoundMatchup.PlayerFourId = pairRoundMatchup.PlayerFour.Id;
                    aevm.RoundMatchup.TableNo = pairRoundMatchup.Table;
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

            //Check if a player is versing themself or on a team with themself
            if (roundMatchup.PlayerOneId == roundMatchup.PlayerTwoId 
             || roundMatchup.PlayerOneId == roundMatchup.PlayerThreeId 
             || roundMatchup.PlayerOneId == roundMatchup.PlayerFourId
             || roundMatchup.PlayerTwoId == roundMatchup.PlayerThreeId
             || roundMatchup.PlayerTwoId == roundMatchup.PlayerFourId
             || roundMatchup.PlayerThreeId == roundMatchup.PlayerFourId)
            {
                TempData["Errors"] = "A player cannot verse themself or be on a team with themself";
                var adminEditViewModel1 = new AdminEditViewModel() {
                    RoundMatchup = roundMatchup,
                    Players = _context.Players.ToList()
                };
                return View(adminEditViewModel1);
            }
            PairRoundMatchup updatedPairRoundMatchup = null;
            RoundMatchup updatedRoundMatchup = null;
            if (_context.RoundMatchups.Find(id) is PairRoundMatchup)
            {
                updatedPairRoundMatchup = _context.RoundMatchups.Include(p => p.PlayerOne).Include(p => p.PlayerTwo).SingleOrDefault(r => r.Id == id) as PairRoundMatchup;
                updatedPairRoundMatchup.PlayerOne = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerOneId);
                updatedPairRoundMatchup.PlayerTwo = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerTwoId);
                updatedPairRoundMatchup.PlayerThree = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerThreeId);
                updatedPairRoundMatchup.PlayerFour = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerFourId);
                updatedPairRoundMatchup.Table = roundMatchup.TableNo;
            } else
            {
                updatedRoundMatchup = _context.RoundMatchups.Include(p => p.PlayerOne).Include(p => p.PlayerTwo).SingleOrDefault(r => r.Id == id);
                updatedRoundMatchup.PlayerOne = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerOneId);
                updatedRoundMatchup.PlayerTwo = _context.Players.FirstOrDefault(p => p.Id == roundMatchup.PlayerTwoId);
                updatedRoundMatchup.Table = roundMatchup.TableNo;
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
            var adminEditViewModel = new AdminEditViewModel()
            {
                RoundMatchup = roundMatchup,
                Players = _context.Players.ToList()
            };
            return View(adminEditViewModel);
        }

        public ActionResult ValidateRoundMatchups()
        {
            List<List<string>> errors = RoundMatchupActions.GetRoundMatchupErrors(_context);
            if (errors[0].Count == 0 && errors[1].Count == 0 && errors[2].Count == 0 && errors[3].Count == 0)
            {
                TempData["Status"] = "You're good to go!";
            }
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
            ViewData["DuplicateOpponents"] = TempData["DuplicateOpponents"];
            ViewData["OverallocatedPlayers"] = TempData["OverallocatedPlayers"];
            ViewData["UnallocatedPlayers"] = TempData["UnallocatedPlayers"];

            var roundMatchups = _context.RoundMatchups.Where(r => !(r is PairRoundMatchup)).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList();
            var pairRoundMatchups = _context.PairRoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).ToList();

            return View(roundMatchups.Union(pairRoundMatchups));
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
                var prmevm = pairRoundMatchup.ToPairRoundMatchupEditViewModel();
                prmevm.Players = players;
                return View("PairRoundMatchupEdit", prmevm);
            }
            else
            {
                var arevm = roundMatchup.ToRoundMatchupEditViewModel();
                arevm.Players = players;
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

            //Check if a player is versing themself or on a team with themself
            if (roundMatchupvm.PlayerOneId == roundMatchupvm.PlayerTwoId)
            {
                TempData["Errors"] = "A player cannot verse themself or be on a team with themself";
                roundMatchupvm.Players = _context.Players.ToList();
                return View(roundMatchupvm);
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
            roundMatchupvm.Players = _context.Players.ToList();
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

            //Check if a player is versing themself or on a team with themself
            if (pairRoundMatchupvm.PlayerOneId == pairRoundMatchupvm.PlayerTwoId
             || pairRoundMatchupvm.PlayerOneId == pairRoundMatchupvm.PlayerThreeId
             || pairRoundMatchupvm.PlayerOneId == pairRoundMatchupvm.PlayerFourId
             || pairRoundMatchupvm.PlayerTwoId == pairRoundMatchupvm.PlayerThreeId
             || pairRoundMatchupvm.PlayerTwoId == pairRoundMatchupvm.PlayerFourId
             || pairRoundMatchupvm.PlayerThreeId == pairRoundMatchupvm.PlayerFourId)
            {
                TempData["Errors"] = "A player cannot verse themself or be on a team with themself";
                pairRoundMatchupvm.Players = _context.Players.ToList();
                return View(pairRoundMatchupvm);
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
            pairRoundMatchupvm.Players = _context.Players.ToList();
            return View(pairRoundMatchupvm);
        }

        public ActionResult ValidateAllRoundMatchups()
        {
            List<List<string>> errors = RoundMatchupActions.GetRoundMatchupErrors( _context);
            if (errors[0].Count == 0 && errors[1].Count == 0 && errors[2].Count == 0 && errors[3].Count == 0)
            {
                TempData["Status"] = "You're good to go!";
            }
            TempData["DuplicateOpponents"] = string.Join(" | ", errors[1]);
            TempData["OverallocatedPlayers"] = string.Join(" | ", errors[2]);
            TempData["UnallocatedPlayers"] = string.Join(" | ", errors[3]);
            return RedirectToAction(nameof(AllRounds));
        }
    }
}