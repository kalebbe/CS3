/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      HomeController.cs
 * @revision:  1.0
 * @synapsis:  This controller sends the user to the page where they can start a new game.
 * 
 *             -----UPDATE FINAL-----
 *             -Removed all Data calls from controller. Calls go through service now.
 */

using MinesweeperMVC.Services.Business;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Method checks if the user haves any saved games then sends them home.
        public ActionResult Index()
        {
            GameService gs = new GameService();
            bool saves = gs.CheckSaves((int)Session["UserId"], false);
            return View("Home", (object)saves.ToString());
        }
    }
}