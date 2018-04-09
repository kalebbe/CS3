/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      StatsController.cs
 * @revision:  1.0
 * @synapsis:  This controller forward the user to the stats page which allows them to view
 *             their game stats. I might add in the ability for them to see the global game
 *             stats here too, but for now that is API only.
 */

using MinesweeperMVC.Models.Data;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class StatsController : Controller
    {
        StatsDAO std = new StatsDAO();
        List<string> stats = new List<string>();
        // GET: Stats
        public ActionResult Index()
        {
            stats = std.GetStats((int)Session["UserId"], "USER_ID");
            return View("Stats", stats);
        }
    }
}