/*
 * @authors:   Kaleb Eberhart
 * @date:      02/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.3
 * @file:      GameController.cs
 * @revision:  1.1
 * @synapsis:  This controller is actually pretty vital. It starts the game and
 *             generates the game board, handles the user's clicks and renders
 *             the partial view containing the actual board. 10/10 controller here.
 *             
 *             ---------UPDATE MILESTONE 4---------
 *             -Added new difficulty to allow saved games.
 *             -Implemented TurnCount to autosave every 5 games.
 *             -Added right click to add flags.
 *             
 *             TODO: Limit flags to amount of bombs and implement flagcount on gui.
 *                   Ajax the right clicks to avoid page refreshes.
 *                   Implement undo button.
 */

using MinesweeperMVC.Models.Business;
using MinesweeperMVC.Models.Data;
//using System.Diagnostics; Used for debugging
using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class GameController : Controller
    {
        private int x = 0;
        public static int Size;
        //public static ButtonModel[,] PrevTurn;
        //public static bool Reverted;
        GameLogic gl;
        GameDAO gd;
        //public static int FlagCount = 0;

        //This method takes the user's difficulty selection and generates the
        //board through the gamelogic class.
        [HttpGet]
        public ActionResult StartGame(int difficulty)
        {
            //This was put at the start of the method because 4 difficulty is
            //for a saved game and cannot use the formula for size. Instead it
            //retrieves size from the database.
            if (difficulty == 4)
            {
                gd = new GameDAO();

                //This method sets the BtnHolder = the stored md array of the saved game.
                Size = gd.SetArray((int)Session["UserId"]);

                return View("Game");
            }
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

        [HttpPost]
        public ActionResult LeftClick(string btn, string direction)
        {
            //PrevTurn = GameLogic.BtnHolder; //This is the array for the undo button that I'm trying to implement.
            //Reverted = false;
            GameLogic.TurnCount++;

            //This debug.writeline statement outputs (0,0) as the same which it is supposed to.
            //Debug.WriteLine("\n\n\n\n\nPrevTurn(0,0) = " + PrevTurn[0, 0].ToString() + ".\nBtnHolder(0,0) = " + GameLogic.BtnHolder[0, 0].ToString());

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
                SaveGame();
            }

            //This debug.writeline is supposed to have a different PrevTurn[0,0] and BtnHolder[0,0], but for some reason the PrevTurn is changing with the BtnHolder....
            //I will figure this out before the next milestone.
            //Debug.WriteLine("\nAfterProcess\n\nPrevTurn(0,0) = " + PrevTurn[0, 0].ToString() + ".\nBtnHolder(0,0) = " + GameLogic.BtnHolder[0, 0].ToString());
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
        public void SaveGame()
        {
            gd = new GameDAO();
            gd.SaveGame((int)Session["UserId"], GameLogic.BtnHolder);
        }
        /*
         * This is a feature I was working on, but I can't get it working yet, so I'm pushing it to the next milestone.
         * Basically, I'd like to allow players to undo their last move, but for some reason the static multidimensional
         * variable that I'm setting is changing on its own. I will figure this out before the next milestone.
        
        [HttpPost]
        public ActionResult Revert()
        {
            Reverted = true;
            GameLogic.Win = false;
            GameLogic.Lose = false;
            GameLogic.BtnHolder = PrevTurn;
            return PartialView("_GameBoard", GameLogic.BtnHolder);
        }
        */
    }
}