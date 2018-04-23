/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      SaveController.cs
 * @revision:  1.0
 * @synapsis:  This controller is used to save games, delete saves, and load the page with saved games.
 *             This was added with the addition of multiple saves + custom saves in milestone 5.
 *             
 *             -----UPDATE FINAL-----
 *             -Removed all Data calls from controller. Calls go through service now.
 */
using System.Collections.Generic;
using System.Web.Mvc;
using MinesweeperMVC.Services.Business;
using MinesweeperMVC.Services.Utilities;

namespace MinesweeperMVC.Controllers
{
    public class SaveController : Controller
    {
        GameService gs = new GameService();
        List<string> saves = new List<string>();

        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Index returns the user's saved games page if they have any.
        public ActionResult Index()
        {
            saves = gs.GetSaves((int)Session["UserId"]);
            return View("Saves", saves);
        }

        [HttpPost]
        [Auth] //Page security. Users must be logged in to view these pages.
        //This method adds a save to the user's save table. Will overwrite the oldest custom save if the user
        //already has 4. They are warned in the view.
        public ActionResult SaveGame()
        {
            gs.SaveGame((int)Session["UserId"], GameLogic.BtnHolder, false, Request.Form["name"]);
            saves = gs.GetSaves((int)Session["UserId"]);

            return View("Saves", saves);
        }

        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Deletes the user's selected save and returns them to the save page. May add confirmation message
        //in the future.
        public ActionResult DeleteSave(int id)
        {
            gs.DeleteSave(id, false);
            saves = gs.GetSaves((int)Session["UserId"]);

            return View("Saves", saves);
        }
    }
}