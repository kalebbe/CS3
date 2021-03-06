﻿/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      StatsDAO.cs
 * @revision:  1.0
 * @synapsis:  This class is used to update stats, create a new player's stats (happens at registration),
 *             and get the stats for the API or view.
 */
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace MinesweeperMVC.Models.Data
{
    [DataContract]
    public class StatsDAO
    {
        private int win, loss, tp, turns = 0;
        private string query;
        private string connString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Minesweeper;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection conn;
        SqlDataReader dr;
        SqlCommand cmd;

        public StatsDAO()
        {
            conn = new SqlConnection(connString);
        }

        //This method is called when the user registers, so they are added to the game statistics for future use. It seemed
        //like a logical place for their slot to be created.
        public void CreatePlayer(int id)
        {
            query = "INSERT INTO dbo.gamestats (USER_ID, TOTAL_WINS, TOTAL_LOSSES, TIME_PLAYED, TURNS) VALUES(@id, '0', '0', '0', '0')";

            conn.Open();
            cmd = new SqlCommand(query, conn);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //This method gets called twice every time a user completes a game. It is called once to update the GLOBAL game stats and then once
        //again to update the user's game stats. These are done in the same data table to avoid having an entire table for 1 row.
        public void UpdateStats(int time, bool result, int turnCount, int id)
        {
            try
            {
                string col;

                //If the id is 0, then it's the global slot being updated. The id for the global slot is 0.
                if (id == 0)
                {
                    col = "ID";
                }
                else
                {
                    col = "USER_ID";
                }
                //Changed to query2 to avoid collusion with GetStats();
                string query2 = "UPDATE dbo.gamestats SET TOTAL_WINS=@wins, TOTAL_LOSSES=@loss, TIME_PLAYED=@tp, TURNS=@turns WHERE " + col + "=@id";

                List<string> list = new List<string>();

                //I grab the user's old stats here in the form of a list so I can add the new stats to them.
                list = GetStats(id, col);

                //The list was stored with comma delimeters, so I split it the same coming out.
                foreach (string split in list)
                {
                    string[] sList = split.Split(',').ToArray();
                    win = int.Parse(sList[0]);
                    loss = int.Parse(sList[1]);
                    tp = int.Parse(sList[2]);
                    turns = int.Parse(sList[3]);
                }
                if (result)
                {
                    win += 1;
                }
                else
                {
                    loss += 1;
                }

                conn.Open();
                cmd = new SqlCommand(query2, conn);
                                
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@wins", SqlDbType.Int).Value = win;
                cmd.Parameters.Add("@loss", SqlDbType.Int).Value = loss;
                cmd.Parameters.Add("@tp", SqlDbType.Int).Value = (tp + time);
                cmd.Parameters.Add("@turns", SqlDbType.Int).Value = (turns + turnCount);

                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch(SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        //This method grabs the id and column (to allow for global stat retrieval) and then returns
        //a list of stats delimited by a comma.
        public List<string> GetStats(int id, string col)
        {

            query = "SELECT * FROM dbo.gamestats WHERE " + col + "=@id";


            cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            conn.Open();

            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                win = (int)dr["TOTAL_WINS"];
                loss = (int)dr["TOTAL_LOSSES"];
                tp = (int)dr["TIME_PLAYED"];
                turns = (int)dr["TURNS"];
            }

            List<string> list = new List<string>
            {
                win + "," + loss + "," + tp + "," + turns
            };
            conn.Close();
            return list;
        }
    }
}