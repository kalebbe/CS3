/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      RegisterController.cs
 * @revision:  1.0
 * @synapsis:  This controller handles the registration of users or sends
 *             them back with error feedback. I can't use ModelState in
 *             the data/business services, so they must be done here or
 *             in the views. I'm working to do that for next milestone.
 *             
 *             TODO: Change data validation to use built in validation.
 *             Will be done with the final milestone. Been putting that off.
 *             
 *             -----UPDATE FINAL-----
 *             -Removed all Data calls from controller. Calls go through service now.
 */

using MinesweeperMVC.Models;
using MinesweeperMVC.Services.Business;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class RegisterController : Controller
    {

        [HttpGet]
        //Method sends user to registration page.
        public ActionResult Index()
        {
            return View("Register");
        }

        [HttpPost]
        //Method checks modelstate, password, username and then registers the user if there are no errors.
        public ActionResult Register(UserModel model)
        {
            UserService us = new UserService(model);
            SecurityService secServ = new SecurityService(model);

            if (!ModelState.IsValid)
            {
                return View();
            }
            if (secServ.ValidatePassword()) //Checks pass length, + regex for letts/nums + matching.
            {
                if (us.CheckExisting()) //Checks if username is taken
                {
                    string hashedPass = secServ.PasswordHasher(); //Retrieves hashed password from db
                    us.Register(hashedPass);
                    return Redirect("../Login"); //TODO: add registration message
                }
                else
                {
                    ModelState.AddModelError("RegError", "Username is already taken!");
                }
            }
            else
            {
                ModelState.AddModelError("RegError", "Password must be 8+ chars, and contain numbers/letters");
            }
            return View(); //User is sent back to register view if anything is wrong.
        }
    }
}