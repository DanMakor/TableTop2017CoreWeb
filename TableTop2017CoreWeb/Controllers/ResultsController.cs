using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TableTop2017CoreWeb.Data;
using TableTop2017CoreWeb.Helpers;
using TableTop2017CoreWeb.Models;

namespace TableTop2017CoreWeb.Controllers
{
    public class ResultsController : Controller
    {
        private readonly TournamentDbContext _context;

        public ResultsController(TournamentDbContext context)
        {
            _context = context;
        }
        // GET: RoundMatchups Results
        public ActionResult Index()
        {
            int lastRoundNo = RoundMatchupActions.GetLastRoundNo(_context);
            var roundMatchups = _context.RoundMatchups.Where(r => !(r is PairRoundMatchup)).Where(r => r.RoundNo == lastRoundNo).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList();
            var pairRoundMatchups = _context.PairRoundMatchups.Where(r => r.RoundNo == lastRoundNo).Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).ToList();

            return View(roundMatchups.Union(pairRoundMatchups));
        }

        //GET: RoundMatchupEdit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchup =  _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).SingleOrDefault(m => m.Id == id);
            var pairRoundMatchup = _context.PairRoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).Include(r => r.PlayerThree).Include(r => r.PlayerFour).SingleOrDefault(m => m.Id == id);
            if (pairRoundMatchup != null)
            {
                var prmevm = (roundMatchup as PairRoundMatchup).ToPairRoundMatchupEditViewModel();
                return View("PairRoundMatchupEdit", prmevm);
            }
            else 
            {
                var arevm = roundMatchup.ToRoundMatchupEditViewModel();
                return View("RoundMatchupEdit", arevm);
            }
        }


        //POST: RoundMatchups Results

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
                return RedirectToAction(nameof(Index));
            }
            return View(roundMatchup);
        }

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
                return RedirectToAction(nameof(Index));
            }
            return View(roundMatchup);
        }

        //Confirm the score inputs and redirect to the player page index
        public ActionResult ConfirmScoreInputs()
        {
            return RedirectToAction("Index", "Players");
        }
    }
}