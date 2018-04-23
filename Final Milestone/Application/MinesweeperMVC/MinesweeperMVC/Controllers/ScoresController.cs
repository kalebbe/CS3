/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      ScoresController.cs
 * @revision:  1.0
 * @synapsis:  This controller is used to return the High scores view with the required data.
 *             
 *             -----UPDATE FINAL-----
 *             -Removed all Data calls from controller. Calls go through service now.
 */
using MinesweeperMVC.Services.Business;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class ScoresController : Controller
    {
        GameService gs = new GameService();
        //This array is 15x5 because there are 15 total highscore entries in each
        //table with 5 pieces of information attached to each.
        string[,] arr = new string[15, 5];

        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Method loads the hiscores page with time highscores as default selected.
        public ActionResult Index()
        {
            Session["Page"] = "time"; //Session variable used to set up page.
            arr = gs.GetScores("dbo.highscorestime");
            return View("HighScores", arr);
        }

        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Method loads the hiscores page with turn highscores as selected.
        public ActionResult Turns()
        {
            Session["Page"] = "turns"; //Session variable used to set up page.
            arr = gs.GetScores("dbo.highscoresturns");
            return View("HighScores", arr);
        }
    }
}