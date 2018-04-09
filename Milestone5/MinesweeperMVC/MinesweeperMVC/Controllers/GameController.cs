/*
 * @authors:   Kaleb Eberhart
 * @date:      02/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
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
 */

using MinesweeperMVC.Models.Business;
using MinesweeperMVC.Models.Data;
using System.Diagnostics;
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class GameController : Controller
    {
        private int x = 0;
        public static int Size;
        GameLogic gl;
        GameDAO gd;
        StatsDAO std;
        ScoresDAO scd;
        public static Stopwatch Timer = new Stopwatch();

        //This method takes the user's difficulty selection and generates the
        //board through the gamelogic class.
        [HttpGet]
        public ActionResult StartGame(int difficulty)
        {
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
        public ActionResult LoadSave(int id)
        {
            GameLogic.SaveGame = true;
            gd = new GameDAO();
            Size = gd.SetArray(id);
            GameLogic.Lose = false;
            GameLogic.Win = false;
            return View("Game");
        }

        [HttpPost]
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
                std = new StatsDAO();
                scd = new ScoresDAO();
                int diff = (Size - 5) / 5;
                if (GameLogic.Win)
                {
                    int place = scd.CheckHigh(diff, Timer.Elapsed.Seconds, GameLogic.TurnCount, "dbo.highscorestime");
                    if (place != 0)
                    {
                        scd.UpdateHigh(diff, Timer.Elapsed.Seconds, GameLogic.TurnCount, "dbo.highscorestime", Session["Username"].ToString(), place);
                    }
                    place = scd.CheckHigh(diff, Timer.Elapsed.Seconds, GameLogic.TurnCount, "dbo.highscoresturns");
                    if (place != 0)
                    {
                        scd.UpdateHigh(diff, Timer.Elapsed.Seconds, GameLogic.TurnCount, "dbo.highscoresturns", Session["Username"].ToString(), place);
                    }
                }

                //This method is called twice so it updates the user stats and universal game stats.
                //Yes this could be done in one call to the method by adding extra sql statements, but
                //it takes up less space this way.
                std.UpdateStats(Timer.Elapsed.Seconds, GameLogic.Win, GameLogic.TurnCount, (int)Session["UserId"]);
                std.UpdateStats(Timer.Elapsed.Seconds, GameLogic.Win, GameLogic.TurnCount, 0);
                gd = new GameDAO();
                gd.DeleteSave((int)Session["UserId"], true);
            }

            return PartialView("_GameBoard", GameLogic.BtnHolder);
        }

        //Method added to handle a user's right click.
        [HttpPost]
        public ActionResult RightClick()
        {
            //Sent in the old fashion way because this was an ajax/script/jquery created form.
            int valueX = int.Parse(Request["xcoor"]);
            int valueY = int.Parse(Request["ycoor"]);

            //Sets the button to flagged which disables left clicking on said button. I will be adding a flag count that ++ here unless it is == bombcount.
            if(!GameLogic.BtnHolder[valueX, valueY].Flagged)
            {
                GameLogic.BtnHolder[valueX, valueY].Flagged = true;
            }
            //Removes the flag if it was already set. See above, but --.
            else
            {
                GameLogic.BtnHolder[valueX, valueY].Flagged = false;
            }
            
            //This part doesn't use ajax yet, so this will refresh the page. Working on a fix for next milestone.
            return View("Game");
        }

        public ActionResult LoadBoard()
        {
            return PartialView("_GameBoard", GameLogic.BtnHolder);
        }

        //Saves the user's game to the database.
        public void SaveGame(bool autoSave, string name)
        {
            gd = new GameDAO();
            gd.SaveGame((int)Session["UserId"], GameLogic.BtnHolder, autoSave, name);
        }
    }
}