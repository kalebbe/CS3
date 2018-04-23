/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      LoginController.cs
 * @revision:  1.2
 * @synapsis:  This class logs the user in if their information is correct or returns
 *             them to the login page with error feedback. Same thing that applied w/
 *             registration applies here.
 *             
 *             -----UPDATE FINAL-----
 *             -Removed all Data calls from controller. Calls go through service now.
 */

using MinesweeperMVC.Models;
using MinesweeperMVC.Services.Business;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        //Method to direct user to login page.
        public ActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        //Method checks user's modelstate and login credentials then logs them in if they're correct
        //or returns an error otherwise.
        public ActionResult Login(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            SecurityService secServ = new SecurityService(model);
            UserService us = new UserService(model);

            if (us.Login())
            {
                Session["Logged"] = true; //Session variable for page security

                //This was added Milestone 4, so I can grab the user's saved games.
                Session["UserId"] = us.GetUserId();

                //Added with milestone 5 to for highscores placement
                Session["Username"] = model.Username;
                bool saves = us.CheckSaves((int)Session["UserId"], false);
                return View("~/Views/Home/Home.cshtml", (object)saves.ToString());
            }
            else
            {
                ModelState.AddModelError("LoginError", "Username/password combination is incorrect!");
                return View();
            }
        }
    }
}