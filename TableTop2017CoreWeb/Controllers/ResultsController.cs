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
            int lastRound = RoundMatchupActions.GetLastRoundNo(_context);
            List<RoundMatchup> roundMatchups = _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList().Where(r => r.RoundNo == lastRound).OrderByDescending(r => r.PlayerOne.BattleScore).ToList();
            return View(roundMatchups);
        }

        //GET: RoundMatchupEdit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roundMatchup =  _context.RoundMatchups.Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList().SingleOrDefault(m => m.Id == id);
            if (roundMatchup is PairRoundMatchup)
                return View("PairRoundMatchupEdit", roundMatchup);
            else
                return View("RoundMatchupEdit", roundMatchup);
        }


        //POST: RoundMatchups Results

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoundMatchupEdit(int id, [Bind("Id,RoundNo,PlayerOneId,PlayerTwoId,PlayerOneBattleScore,PlayerTwoBattleScore,PlayerOneSportsmanshipScore,PlayerTwoSportsmanshipScore,Table")] RoundMatchup roundMatchup)
        {
            if (id != roundMatchup.Id)
            {
                return NotFound();
            }

            var oldRoundMatchup = _context.RoundMatchups.SingleOrDefault(r => r.Id == roundMatchup.Id);
            roundMatchup.PlayerOne = oldRoundMatchup.PlayerOne;
            roundMatchup.PlayerTwo = oldRoundMatchup.PlayerTwo;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roundMatchup);
                    await _context.SaveChangesAsync();
                    PlayerActions.SetPlayerScores(roundMatchup.PlayerOne.Id, _context);
                    PlayerActions.SetPlayerScores(roundMatchup.PlayerTwo.Id, _context);
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
        public async Task<IActionResult> PairRoundMatchupEdit(int id, [Bind("Id,RoundNo,PlayerOneBattleScore,PlayerTwoBattleScore,PlayerThreeBattleScore,PlayerFourBattleScore,PlayerOneSportsmanshipScore,PlayerTwoSportsmanshipScore,PlayerThreeSportsmanshipScore,PlayerFourSportsmanshipScore,Table")] PairRoundMatchup pairRoundMatchup)
        {
            if (id != pairRoundMatchup.Id)
            {
                return NotFound();
            }

            var oldPairRoundMatchup = _context.RoundMatchups.AsNoTracking().Include(r => r.PlayerOne).Include(r => r.PlayerTwo).ToList().SingleOrDefault(r => r.Id == pairRoundMatchup.Id) as PairRoundMatchup;
            pairRoundMatchup.PlayerOne = oldPairRoundMatchup.PlayerOne;
            pairRoundMatchup.PlayerTwo = oldPairRoundMatchup.PlayerTwo;
            pairRoundMatchup.PlayerThree = oldPairRoundMatchup.PlayerThree;
            pairRoundMatchup.PlayerThree = oldPairRoundMatchup.PlayerFour;
            //Test with the actual playerthree and four objects there
            //Test with the actual playerthree and four objects there
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pairRoundMatchup);
                    await _context.SaveChangesAsync();
                    PlayerActions.SetPlayerScores(pairRoundMatchup.PlayerOne.Id, _context);
                    PlayerActions.SetPlayerScores(pairRoundMatchup.PlayerTwo.Id, _context);
                    PlayerActions.SetPlayerScores(pairRoundMatchup.PlayerThree.Id, _context);
                    PlayerActions.SetPlayerScores(pairRoundMatchup.PlayerFour.Id, _context);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoundMatchupActions.RoundMatchupsExists(pairRoundMatchup.Id, _context))
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
            return View(pairRoundMatchup);
        }

        //Confirm the score inputs and redirect to the player page index
        public ActionResult ConfirmScoreInputs()
        {
            return RedirectToAction("Index", "Players");
        }
    }
}