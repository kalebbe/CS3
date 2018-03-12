/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.3
 * @file:      HomeController.cs
 * @revision:  1.0
 * @synapsis:  This controller sends the user to the page where they can start a new game.
 */

using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            return View("Home");
        }
    }
}