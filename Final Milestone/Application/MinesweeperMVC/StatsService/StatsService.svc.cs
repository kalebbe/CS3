/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      StatsService.svc.cs
 * @revision:  1.0
 * @synapsis:  This is a service created to show those with access the highscores
 *             and the overall game stats.
 *             
 *             -----UPDATE FINAL-----
 *             -Updated to return json results.
 *             -Removed DAO calls. All calls now go through Services.
 */
using MinesweeperMVC.Services.Business;
using System;
using System.Collections.Generic;

namespace StatsService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class StatsService : IStatsService
    {
        GameService gs = new GameService();

        public DTO GetStats()
        {
            List<string> stats = gs.GetStats(0, "ID");

            var ok = stats.Count > 0; //Will return error if no results
            var errorCode = ok ? 200 : 404;
            var statusMessage = ok ? "Success" : "No Results";

            var dto = new DTO(errorCode, statusMessage, stats);

            return dto;
        }

        //Gets the high scores from the database for fastest games.
        public DTO GetTimeScores()
        {
            string[,] scores = gs.GetScores("dbo.highscorestime");
            List<string> newScores = new List<string>(75);

            //This and the loop is used to turn the array to a list.
            int size1 = scores.GetLength(1);
            int size0 = scores.GetLength(0);

            for (int i = 0; i < size0; i++)
            {
                for (int j = 0; j < size1; j++)
                {
                    newScores.Add(scores[i, j]);
                }
            }

            var ok = newScores.Count > 0; //Will return error if no results
            var errorCode = ok ? 200 : 404;
            var statusMessage = ok ? "Success" : "No Results";

            var dto = new DTO(errorCode, statusMessage, newScores);

            return dto;
        }
        
        //Gets the high scores from the database for least turns in a game.
        public DTO GetTurnsScores()
        {
            string[,] scores = gs.GetScores("dbo.highscoresturns");
            List<string> newScores = new List<string>(75);

            int size1 = scores.GetLength(1);
            int size0 = scores.GetLength(0);

            for(int i = 0; i < size0; i++)
            {
                for(int j = 0; j < size1; j++)
                {
                    newScores.Add(scores[i, j]);
                }
            }

            var ok = newScores.Count > 0; //Will return error if no results
            var errorCode = ok ? 200 : 404;
            var statusMessage = ok ? "Success" : "No Results";

            var dto = new DTO(errorCode, statusMessage, newScores);

            return dto;
        }
    }
}
