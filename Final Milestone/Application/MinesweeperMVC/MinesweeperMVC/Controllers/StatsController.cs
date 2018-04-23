/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      StatsController.cs
 * @revision:  1.0
 * @synapsis:  This controller forward the user to the stats page which allows them to view
 *             their game stats. I might add in the ability for them to see the global game
 *             stats here too, but for now that is API only.
 *             
 *             -----UPDATE FINAL-----
 *             -Removed all Data calls from controller. Calls go through service now.
 */

using MinesweeperMVC.Services.Business;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class StatsController : Controller
    {
        GameService gs = new GameService();
        List<string> stats = new List<string>();
        
        
        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Method returns stats page with a list of stats as the model.
        public ActionResult Index()
        {
            stats = gs.GetStats((int)Session["UserId"], "USER_ID");
            return View("Stats", stats);
        }
    }
}