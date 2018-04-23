/*
 * @authors:   Kaleb Eberhart
 * @date:      04/22/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      GameService.cs
 * @revision:  1.0
 * @synapsis:  This class was created to incorporate layered architecture in
 *             my project. Controllers no longer call the data layer directly.
 *             Handles all services required with the game.
 */

using MinesweeperMVC.Controllers;
using MinesweeperMVC.Models;
using MinesweeperMVC.Services.Data;
using MinesweeperMVC.Services.Utilities;
using System.Collections.Generic;

namespace MinesweeperMVC.Services.Business
{
    public class GameService
    {
        private GameDAO gd;
        private StatsDAO std;
        private ScoresDAO scd;

        //Constructor instantiates 3 DAOs
        public GameService()
        {
            gd = new GameDAO();
            std = new StatsDAO();
            scd = new ScoresDAO();
        }

        //Calls the SetArray method in GameDAO to get the required size of the gameboard.
        public int GetSize(int id)
        {
            MineLogger.getInstance().Info("GetSize method invoked in GameService class.");
            return gd.SetArray(id);
        }

        //Checks highscores to see if user has a new high schore then updates the highscores
        //if they do.
        public void WinLogic(int diff, string username)
        {
            MineLogger.getInstance().Info("WinLogic method invoked in GameService class.");

            //Place check high and update high are ran twice because both tables need to be checked
            int place = scd.CheckHigh(diff, GameController.Timer.Elapsed.Seconds, 
                GameLogic.TurnCount, "dbo.highscorestime");
            if(place != 0)
            {
                scd.UpdateHigh(diff, GameController.Timer.Elapsed.Seconds, GameLogic.TurnCount,
                    "dbo.highscorestime", username, place);
            }

            place = scd.CheckHigh(diff, GameController.Timer.Elapsed.Seconds,
                GameLogic.TurnCount, "dbo.highscoresturns");
            if (place != 0)
            {
                scd.UpdateHigh(diff, GameController.Timer.Elapsed.Seconds, GameLogic.TurnCount,
                    "dbo.highscoresturns", username, place);
            }
        }

        //Updates Global game stats and user's game stats at the end of the game.
        public void GameEnd(int id)
        {
            MineLogger.getInstance().Info("GameEnd method invoked in GameService class.");
            std.UpdateStats(GameController.Timer.Elapsed.Seconds, GameLogic.Win, GameLogic.TurnCount, id);
            std.UpdateStats(GameController.Timer.Elapsed.Seconds, GameLogic.Win, GameLogic.TurnCount, 0);
            DeleteSave(id, true);
        }

        //Calls the GameDAO to save the user's game.
        public void SaveGame(int id, ButtonModel[,] bm, bool autoSave, string name)
        {
            MineLogger.getInstance().Info("SaveGame method invoked in GameService class.");
            gd.SaveGame(id, bm, autoSave, name);
        }

        //Calls the GameDAO to check if the user has any saves.
        public bool CheckSaves(int id, bool auto)
        {
            MineLogger.getInstance().Info("CheckSaves method invoked in GameService class.");
            return gd.CheckSaves(id, auto);
        }

        //Calls the GameDAO to grab the user's saves.
        public List<string> GetSaves(int id)
        {
            MineLogger.getInstance().Info("GetSaves method invoked in GameService class.");
            return gd.GetSaves(id);
        }

        //Calls the GameDAO to delete a certain save.
        public void DeleteSave(int id, bool auto)
        {
            MineLogger.getInstance().Info("DeleteSave method invoked in GameService class");
            gd.DeleteSave(id, auto);
        }

        //Calls the ScoresDAO to return the high scores.
        public string[,] GetScores(string table)
        {
            MineLogger.getInstance().Info("GetScores method invoked in GameService class");
            return scd.GetScores(table);
        }

        //Calls the StatsDAO to return the game stats (of user).
        public List<string> GetStats(int id, string col)
        {
            MineLogger.getInstance().Info("GetStats method invoked in GameService class");
            return std.GetStats(id, col);
        }
    }
}