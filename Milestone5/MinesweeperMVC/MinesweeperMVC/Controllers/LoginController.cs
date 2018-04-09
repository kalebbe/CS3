/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      LoginController.cs
 * @revision:  1.2
 * @synapsis:  This class logs the user in if their information is correct or returns
 *             them to the login page with error feedback. Same thing that applied w/
 *             registration applies here.
 */

using MinesweeperMVC.Models;
using MinesweeperMVC.Models.Business;
using MinesweeperMVC.Models.Data;
using System;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View("Login");
        }

        //Again, we're missing some layered architecture here, we'll get to it ASAP
        [HttpPost]
        public ActionResult Login(UserModel model)
        {
            SecurityService secServ = new SecurityService(model);
            SecurityDAO sd = new SecurityDAO(model);
            UserDAO ud = new UserDAO(model);
            GameDAO gd = new GameDAO();
            String hashPass = sd.GetPass(); //Hashed password must be retrieved from db
            if (secServ.CheckHash(hashPass)) //Hash is checked with user input
            {
                //This may be changed later to a database variable if needed.
                Session["Logged"] = true;

                //This was added Milestone 4, so I can grab the user's saved games.
                Session["UserId"] = ud.GetUserId();

                //Added with milestone 5 to for highscores placement
                Session["Username"] = model.Username;
                bool saves = gd.CheckSaves((int)Session["UserId"], false);
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