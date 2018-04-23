/*
 * @authors:   Kaleb Eberhart
 * @date:      02/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      LogoutController.cs
 * @revision:  1.0
 * @synapsis:  All this controller does is clear the user's session data
 *             and sends them back to the login screen.
 */
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class LogoutController : Controller
    {
        [HttpGet]
        //Method kills the session and redirects the user to the home(login) page.
        public ActionResult Index()
        {
            Session.Abandon();
            return View("~/Views/Login/Login.cshtml");
        }
    }
}