/*
 * @authors:   Kaleb Eberhart
 * @date:      04/08/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      ScoresDAO.cs
 * @revision:  1.0
 * @synapsis:  This class is used to check the highscores, update the highscores and retrieve the data
 *             from the highscores.
 */
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MinesweeperMVC.Models.Data
{
    //Not sure this is necesarry for the API, but this class is used.
    [DataContract]
    public class ScoresDAO
    {
        private string query, query2;
        private string connString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Minesweeper;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection conn, conn2;
        SqlDataReader dr;
        SqlCommand cmd, cmd2;

        public ScoresDAO()
        {
            //2 connections used here due to multiple querys in update
            conn = new SqlConnection(connString);
            conn2 = new SqlConnection(connString);
        }

        //This method is basically only used to check if the user's time or turns beats any of the 
        //scores currently in the high scores. After it will update the highscores in another method
        //if they'ver surpassed an old score.
        public int CheckHigh(int diff, int time, int turns, string table)
        {
            try
            {
                //Ordered as such to update all higher scores when a new score is achieved. There are 2 separate
                //tables because there are 2 different types of high scores.
                query = "SELECT * FROM " + table + " WHERE DIFFICULTY=@diff ORDER BY PLACE DESC";
                int place = 0;
                conn.Open();
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@diff", SqlDbType.Int).Value = diff;
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    //In here I'm checking if the user's new time/turns is less than any old time/turns in the database.
                    if (table == "dbo.highscorestime")
                    {
                        int oldTime = (int)dr["GAME_TIME"];
                        if (oldTime > time)
                        {
                            place = (int)dr["PLACE"];
                        }
                    }
                    else
                    {
                        int oldTurns = (int)dr["TURNS"];
                        if (oldTurns > turns)
                        {
                            place = (int)dr["PLACE"];
                        }
                    }

                }
                conn.Close();
                return place;
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        //This method is called after the CheckHigh method if the user's new score has beaten an old score. This method
        //required multiple queries to accomplish its mission.
        public void UpdateHigh(int diff, int time, int turns, string table, string username, int place)
        {
            try
            {
                Int32 id = 0;

                //Before anything else happens, the new high score is inserted into the database with a new id. OUTPUT INSERTED.ID pulls out the newly created id.
                query = "INSERT INTO " + table + " (USERNAME, GAME_TIME, TURNS, DIFFICULTY, PLACE) OUTPUT INSERTED.ID VALUES (@user, @time, @turns, @diff, @place)";
                conn.Open();
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@user", SqlDbType.NVarChar).Value = username;
                cmd.Parameters.Add("@time", SqlDbType.Int).Value = time;
                cmd.Parameters.Add("@turns", SqlDbType.Int).Value = turns;
                cmd.Parameters.Add("@diff", SqlDbType.Int).Value = diff;
                cmd.Parameters.Add("@place", SqlDbType.Int).Value = place;

                //ExecuteScalar allows me to capture the newly created ID for use in the another query.
                id = (Int32)cmd.ExecuteScalar();

                //Grabs all the current high scores (included the newly inserted)
                query = "SELECT * FROM " + table + " WHERE DIFFICULTY=@diff ORDER BY PLACE ASC";
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@diff", SqlDbType.Int).Value = diff;
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if ((int)dr["PLACE"] >= place)
                    {
                        //2nd query to update all high scores that are higher or == the high score I just inserted.
                        query2 = "UPDATE " + table + " SET PLACE=@place WHERE ID=@id";

                        conn2.Open();
                        cmd2 = new SqlCommand(query2, conn2);

                        cmd2.Parameters.Add("@place", SqlDbType.Int).Value = ((int)dr["PLACE"] + 1);
                        cmd2.Parameters.Add("@id", SqlDbType.Int).Value = (int)dr["ID"];

                        //This if stops the execution if the id == the newly inserted id above. Don't want to bump the newby.
                        if ((int)dr["ID"] != id)
                        {
                            cmd2.ExecuteReader();
                        }
                        conn2.Close();
                    }
                    
                    //Deletes the highscore that is bumped up to 6th place.
                    query2 = "DELETE FROM " + table + " WHERE PLACE='6'";
                    conn2.Open();
                    cmd2 = new SqlCommand(query2, conn2);

                    cmd2.ExecuteReader();
                    conn2.Close();
                }
                conn.Close();
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        //Method that grabs the highscores and puts them into a multidimensional array
        //for the API or view with the highscores.
        public string[,] GetScores(string table)
        {
            query = "SELECT * FROM " + table + " ORDER BY DIFFICULTY, PLACE ASC";

            cmd = new SqlCommand(query, conn);
            conn.Open();

            dr = cmd.ExecuteReader();
            string[,] arr = new string[15, 5];
            int i = 0;

            //This creates the 15 rows of data in the md array
            while (dr.Read())
            {
                arr[i, 0] = dr["USERNAME"].ToString();
                arr[i, 1] = dr["GAME_TIME"].ToString();
                arr[i, 2] = dr["TURNS"].ToString();
                arr[i, 3] = dr["DIFFICULTY"].ToString();
                arr[i, 4] = dr["PLACE"].ToString();
                i++;
            }
            conn.Close();
            return arr;
        }
    }
}