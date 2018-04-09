/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      HomeController.cs
 * @revision:  1.0
 * @synapsis:  This controller sends the user to the page where they can start a new game.
 */

using MinesweeperMVC.Models.Data;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            GameDAO gd = new GameDAO();
            bool saves = gd.CheckSaves((int)Session["UserId"], false);
            return View("Home", (object)saves.ToString());
        }
    }
}