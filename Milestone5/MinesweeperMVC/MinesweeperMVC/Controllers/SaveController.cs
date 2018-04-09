/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      SaveController.cs
 * @revision:  1.0
 * @synapsis:  This controller is used to save games, delete saves, and load the page with saved games.
 *             This was added with the addition of multiple saves + custom saves in milestone 5.
 */
using MinesweeperMVC.Models.Business;
using MinesweeperMVC.Models.Data;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class SaveController : Controller
    {
        GameDAO gd = new GameDAO();
        List<string> saves = new List<string>();

        // GET: Save
        public ActionResult Index()
        {
            saves = gd.GetSaves((int)Session["UserId"]);
            return View("Saves", saves);
        }

        //This adds a save to the user's save table. Will overwrite the oldest custom save if the user
        //already has 4. They are warned in the view.
        [HttpPost]
        public ActionResult SaveGame()
        {
            gd.SaveGame((int)Session["UserId"], GameLogic.BtnHolder, false, Request.Form["name"]);
            saves = gd.GetSaves((int)Session["UserId"]);

            return View("Saves", saves);
        }

        //Deletes the user's selected save and returns them to the save page. May add confirmation message
        //in the future.
        [HttpGet]
        public ActionResult DeleteSave(int id)
        {
            gd.DeleteSave(id, false);
            saves = gd.GetSaves((int)Session["UserId"]);

            return View("Saves", saves);
        }
    }
}