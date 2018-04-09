/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      ScoresController.cs
 * @revision:  1.0
 * @synapsis:  This controller is used to return the High scores view with the required data.
 */
using MinesweeperMVC.Models.Data;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class ScoresController : Controller
    {
        ScoresDAO scd = new ScoresDAO();
        string[,] arr = new string[15, 5];
        // GET: Scores
        public ActionResult Index()
        {
            Session["Page"] = "time";
            arr = scd.GetScores("dbo.highscorestime");
            return View("HighScores", arr);
        }

        //These are separated because I have to different high scores tables. One for time and one for turns.
        public ActionResult Turns()
        {
            Session["Page"] = "turns";
            arr = scd.GetScores("dbo.highscoresturns");
            return View("HighScores", arr);
        }
    }
}