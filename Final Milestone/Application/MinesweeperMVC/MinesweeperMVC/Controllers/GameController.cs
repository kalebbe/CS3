/*
 * @authors:   Kaleb Eberhart
 * @date:      02/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      GameController.cs
 * @revision:  1.2
 * @synapsis:  This controller is actually pretty vital. It starts the game and
 *             generates the game board, handles the user's clicks and renders
 *             the partial view containing the actual board. 10/10 controller here.
 *             
 *             ---------UPDATE MILESTONE 4---------
 *             -Added new difficulty to allow saved games.
 *             -Implemented TurnCount to autosave every 5 games.
 *             -Added right click to add flags.
 *             
 *                   
 *             ---------UPDATE MILESTONE 5---------
 *             -Implemented high scores and stats, these are called from here when
 *              a game is ended.
 *             -Added a LoadSave method so users can load their saved game.
 *             
 *             TODO: Stop users from competing in the high scores with saved games.
 *              This is because the users can get around the system using saved games.
 *              I could alos implement security measures to stop this from happeniing
 *              and may do that instead.
 *              
 *             -----UPDATE FINAL-----
 *             -Removed all Data calls from controller. Calls go through service now.
 */

using MinesweeperMVC.Services.Business;
using MinesweeperMVC.Services.Utilities;
using System.Diagnostics;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class GameController : Controller
    {
        private int x = 0;
        public static int Size;
        private GameLogic gl;
        private GameService gs;
        public static Stopwatch Timer = new Stopwatch();

        //This method takes the user's difficulty selection and generates the
        //board through the gamelogic class.
        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        public ActionResult StartGame(int difficulty)
        {
            //All variables reset to false to allow game start.
            GameLogic.SaveGame = false;
            GameLogic.Lose = false;
            GameLogic.Win = false;

            //This isn't magic, just a jenky formula for getting board size
            //instead of using an if/else.
            x = (difficulty * 5) + 5;
            Size = x;

            gl = new GameLogic();
            gl.GenerateBoard();

            //TurnCount was added for the purpose of autosave.
            GameLogic.TurnCount = 0;
            return View("Game");
        }

        [HttpGet]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Method loads a saved game for the user
        public ActionResult LoadSave(int id)
        {
            GameLogic.SaveGame = true;
            gs = new GameService();
            Size = gs.GetSize(id);
            GameLogic.Lose = false;
            GameLogic.Win = false;
            return View("Game");
        }

        [HttpPost]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Method pretty much controls all game logic.
        public ActionResult LeftClick(string btn, string direction)
        {
            if(GameLogic.TurnCount == 0)
            {
                Timer.Reset();
                Timer.Start();
            }
            GameLogic.TurnCount++;

            //I realize this is a ghetto work-a-round, but I was kind of stumped
            //with how to push the object from the view to controller. Will update
            //once we know how to do it the right way.
            string[] coordinates = btn.Split(null);
            int x = int.Parse(coordinates[0]);
            int y = int.Parse(coordinates[1]);

            GameLogic.ProcessCell(x, y);
            GameLogic.CheckWin();

            //Logic that allows the user's game to autosave every 5 turns
            if (GameLogic.TurnCount % 5 == 0 && !GameLogic.Win && !GameLogic.Lose)
            {
                SaveGame(true, "Autosave");
            }
            
            //Deleting autosave if user wins or loses
            if(GameLogic.Win || GameLogic.Lose)
            {
                Timer.Stop();
                gs = new GameService();
                int diff = (Size - 5) / 5;

                //Saved games are being excluded from highscores here
                //because there's no way for me to set a timer with a
                //starting time (that I know of). This stops users from
                //cheating to get into the high scores.
                if (GameLogic.Win && !GameLogic.SaveGame)
                {
                    gs.WinLogic(diff, Session["Username"].ToString());
                }
                gs.GameEnd((int)Session["UserId"]); //This adds game stats to current
            }

            return PartialView("_GameBoard", GameLogic.BtnHolder);
        }

        [HttpPost]
        [Auth] //Page security. Users must be logged in to view these pages.
        //Method added to handle a user's right click.
        public ActionResult RightClick()
        {
            //Sent in the old fashion way because this was an ajax/script/jquery created form.
            int valueX = int.Parse(Request["xcoor"]);
            int valueY = int.Parse(Request["ycoor"]);

            //Sets the button to flagged which disables left clicking on said button.
            if(!GameLogic.BtnHolder[valueX, valueY].Flagged)
            {
                GameLogic.BtnHolder[valueX, valueY].Flagged = true;
            }
            //Removes the flag if it was already set. See above, but --.
            else
            {
                GameLogic.BtnHolder[valueX, valueY].Flagged = false;
            }
            
            //This part doesn't use ajax. Right clicks will cause page refreshes.
            return View("Game");
        }

        [Auth] //Page security. Users must be logged in to view these pages.
        //Method loads the board in the Game view.
        public ActionResult LoadBoard()
        { 
            return PartialView("_GameBoard", GameLogic.BtnHolder);
        }

        [Auth] //Page security. Users must be logged in to view these pages.
        //Saves the user's game to the database.
        public void SaveGame(bool autoSave, string name)
        {
            gs = new GameService();
            gs.SaveGame((int)Session["UserId"], GameLogic.BtnHolder, autoSave, name);
        }
    }
}