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
        public IActionResult Index()
        {
            return View();
        }

        public async Task<int> GetLastRound(TournamentDbContext _context)
        {
            int currentRound = 1;
            Round lastRound = await _context.Rounds.LastOrDefaultAsync();
            if (lastRound != null)
            {
                currentRound = lastRound.RoundNo;
            }
            return currentRound;
        }
    }
}