/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      StatsService.svc.cs
 * @revision:  1.0
 * @synapsis:  This is a service created to show those with access the highscores
 *             and the overall game stats. This will hopefully be reformatted in the
 *             future because the output looks really ugly in the browser. Not entirely
 *             sure how to use line breaks in the format because </br>, Environment.NewLine,
 *             \n, and \r\n didn't work.
 */
using MinesweeperMVC.Models.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StatsService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class StatsService : IStatsService
    {
        StatsDAO std = new StatsDAO();
        ScoresDAO scd = new ScoresDAO();
        string user, user2 = "";
        int win, loss, tp, turns, diff, place, tp2, turns2, diff2, place2 = 0;

        public string GetStats()
        {
            List<string> stats = new List<string>();
            stats = std.GetStats(0, "ID");

            //Splits the list returned into useable strings.
            foreach (string split in stats)
            {
                string[] splitStats = split.Split(',').ToArray();
                win = int.Parse(splitStats[0]);
                loss = int.Parse(splitStats[1]);
                tp = int.Parse(splitStats[2]);
                turns = int.Parse(splitStats[3]);
            }
            int mins = (tp / 60);
            int hours = (mins / 60);
            int secs = (tp % 60);
            //Seriously failed trying to make this pretty because all formatting I tried
            //wouldn't work. Going to push formatting to a future milestone.
            string s = "----------Game Stats---------- @Total wins: " 
                + win + " @Total Losses: " + loss + " @Total time played: " + hours + ":" +
                mins + ":" + secs + " @Total turns: " + turns;

            return s;
        }

        //This method also prints out very ugly and I intend to fix the formatting if possible.
        public string GetHighscores()
        {
            string[,] arrTime = new string[15, 5];
            string[,] arrTurns = new string[15, 5];

            arrTime = scd.GetScores("dbo.highscorestime");
            arrTurns = scd.GetScores("dbo.highscoresturns");

            Debug.WriteLine(arrTime[4, 2]);
            string s = "----------High Scores time----------";
            for(int i = 0; i < 15; i++)
            {
                s = s + " @Username: " + arrTime[i, 0] + " @Time: " + arrTime[i, 1] + " @Turns: " + arrTime[i, 2] +
                    " @Difficulty: " + arrTime[i, 3] + " @Place: " + arrTime[i, 4];
            }
            s = s + "  ----------High Scores Turns----------";
            for(int i = 0; i < 15; i++)
            {
                s = s + " @Username: " + arrTurns[i, 0] + " @Time: " + arrTurns[i, 1] + " @Turns: " + arrTurns[i, 2] +
                    " @Difficulty: " + arrTurns[i, 3] + " @Place: " + arrTurns[i, 4];
            }
            return s;
        }

        //This was added to allow user data retrieval in future milestones. For now it is not in use.
        public DTO GetUser(string id)
        {
            List<string> list = new List<string>();
            DTO dto = new DTO(0, "OK", list);
            return dto;
        }
    }
}
