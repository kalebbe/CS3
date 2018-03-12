/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.3
 * @file:      RegisterController.cs
 * @revision:  1.0
 * @synapsis:  This controller handles the registration of users or sends
 *             them back with error feedback. I can't use ModelState in
 *             the data/business services, so they must be done here or
 *             in the views. I'm working to do that for next milestone.
 *             
 *             TODO: Change data validation to use built in validation.
 *             Would've done that this milestone, but ran out of time.
 */

using MinesweeperMVC.Models;
using MinesweeperMVC.Models.Business;
using MinesweeperMVC.Models.Data;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class RegisterController : Controller
    {

        // GET: Register
        [HttpGet]
        public ActionResult Index()
        {
            return View("Register");
        }

        //Layered architecture has not been fully implemented here.
        //I'm sorry, but we ran out of time getting the game logic linked.
        [HttpPost]
        public ActionResult Register(UserModel model)
        {
            SecurityService secServ = new SecurityService(model);
            SecurityDAO sd = new SecurityDAO(model);
            UserDAO ud = new UserDAO(model);

            if (secServ.ValidatePassword()) //Checks pass length, + regex for letts/nums + matching.
            {
                if (sd.CheckExisting()) //Checks if username is taken
                {
                    string hashedPass = secServ.PasswordHasher(); //Retrieves hashed password from db
                    if (ud.Register(hashedPass)) //Attempts user registry with hashed pass
                    {
                        return Redirect("../Login"); //TODO: add registration message
                    }
                    else
                    {
                        //This should never be reached ideally
                        ModelState.AddModelError("RegError", "Unable to register user, database error");
                    }
                }
                else
                {
                    ModelState.AddModelError("UsernameError", "Username is already taken!");
                }
            }
            else
            {
                ModelState.AddModelError("PasswordError", "Password must be 8+ chars, and contain numbers/letters");
            }
            return View(); //User is sent back to register view if anything is wrong.
        }
    }
}