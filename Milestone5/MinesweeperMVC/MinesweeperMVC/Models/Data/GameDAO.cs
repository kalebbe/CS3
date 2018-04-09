/*
 * @authors:   Kaleb Eberhart
 * @date:      03/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      GameDAO.cs
 * @revision:  1.0
 * @synapsis:  This class is used to save the user's game, delete it, and retrieve it. Will
 *             be updated with the next milestone to allow multiple (custom) saves.
 *             
 *             ---------UPDATE MILESTONE 5--------
 *             -Reworked entirely to allow for multiple saves and custom saves.
 *             -Autosave and name column added to database to allow naming and keeping
 *              track of custom/autosave.
 */

using MinesweeperMVC.Controllers;
using MinesweeperMVC.Models.Business;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MinesweeperMVC.Models.Data
{
    public class GameDAO
    {
        private string connString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Minesweeper;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlConnection conn;
        SqlDataReader dr;
        SqlCommand cmd;

        public GameDAO()
        {
            conn = new SqlConnection(connString);
        }

        public bool SaveGame(int userId, ButtonModel[,] bm, bool autoSave, string name)
        {
            try
            {
                string query;

                //Checks to see if the user already has a saved game and then if the save they're trying to insert is an autosave. 
                //If it is, the old autosave is updated.
                if (CheckSaves(userId, true) && autoSave == true)
                {
                    query = "UPDATE dbo.game SET GAME_BOARD=@game, SAVE_DATE_TIME=@dt, SIZE=@size, AUTOSAVE=@auto, NAME=@name WHERE USER_ID=@id";
                }
                //Inserts a new save into the database because user hasn't reached their limit. This may need to be reordered.
                else if(CountSaves(userId) < 4)
                {
                    query = "INSERT INTO dbo.game (USER_ID, GAME_BOARD, SAVE_DATE_TIME, SIZE, AUTOSAVE, NAME) VALUES(@id, @game, @dt, @size, @auto, @name)";
                }
                else
                {
                    //Updates the autosave if the user currently has one.
                    if (autoSave == true)
                    {
                        query = "UPDATE dbo.game SET GAME_BOARD=@game, SAVE_DATE_TIME=@dt, SIZE=@size, AUTOSAVE=@auto, NAME=@name WHERE USER_ID=@id AND AUTOSAVE='true'";
                    }
                    //Replaces the oldest custom save the user has with their new one.
                    else
                    {
                        userId = GetOldest(userId);
                        query = "UPDATE dbo.game SET GAME_BOARD=@game, SAVE_DATE_TIME=@dt, SIZE=@size, AUTOSAVE=@auto, NAME=@name WHERE ID=@id";
                    }
                }

                conn.Open();

                //This converter allows me to serialize and deserialize Multidimensional arrays. Huge life saver after trying the script serializer.
                string json = JsonConvert.SerializeObject(bm, Formatting.Indented);

                cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@game", SqlDbType.VarChar).Value = json;
                cmd.Parameters.Add("@dt", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@size", SqlDbType.Int).Value = GameController.Size;
                cmd.Parameters.Add("@auto", SqlDbType.NVarChar).Value = autoSave;
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;

                int rows = cmd.ExecuteNonQuery();

                if (rows == 1)
                {
                    conn.Close();
                    return true;
                }
                else
                {
                    conn.Close();
                    return false;
                }
            }
            catch(SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        public int CountSaves(int id)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM dbo.game WHERE USER_ID=@id AND AUTOSAVE='false'";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                conn.Open();
                int saves = (Int32)cmd.ExecuteScalar();
                conn.Close();
                return saves;
            }
            catch(SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        //This method is being used to find the oldest current save
        public int GetOldest(int id)
        {
            try
            {
                int gameId = 0;

                //Ordering the query by date desc allows me to get the oldest save last which is what I need.
                string query = "SELECT * FROM dbo.game WHERE USER_ID=@id ORDER BY SAVE_DATE_TIME DESC";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    gameId = (int)dr["ID"];
                }
                conn.Close();
                return gameId;
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e);
                throw e;
            }
        }

        //This method was added to check if the user has a saved game already to help with saving.
        //Method has been updated with milestone 5 to check for custom and autosaves.
        public bool CheckSaves(int id, bool auto)
        {
            try
            {
                string query;
                if (auto)
                {
                    query = "SELECT * FROM dbo.game WHERE USER_ID=@id AND AUTOSAVE='true'";
                }
                else
                {
                    query = "SELECT * FROM dbo.game WHERE USER_ID=@id";
                }

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    conn.Close();
                    return true;
                }
                else
                {
                    conn.Close();
                    return false;
                }
            }
            catch(SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        //This method gets the Serialized array from the database and then deserializes it and sets the BtnHolder = to it.
        //It returns the size of the board because that is needed to load the board.
        public int SetArray(int id)
        {
            int size = 0;
            try
            {
                string serArray = "";
                string query = "SELECT * FROM dbo.game WHERE ID=@id";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    serArray = (string)dr["GAME_BOARD"];
                    size = (int)dr["SIZE"];
                }

                //Again, the jsonconvert is amazing because I can just reconvert to a md array without doing anything extra.
                //Spent way too much time trying to get this working with the javascript converter.
                ButtonModel[,] bm = JsonConvert.DeserializeObject<ButtonModel[,]>(serArray);
                GameLogic.BtnHolder = bm;
                conn.Close();
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
            return size;
        }

        //This may later be changed to a bool, just depends. Not sure
        //what error I would send to a user for this. This will need
        //to be changed with the next milestone because of multiple saves.
        public void DeleteSave(int id, bool auto)
        {
            try
            {
                string query;

                //This if uses the optional parameter for deleting non-autosave games.
                if(!auto)
                {
                    query = "Delete FROM dbo.game WHERE ID=@id";
                    
                }
                else
                {
                    query = "DELETE FROM dbo.game WHERE USER_ID=@id AND AUTOSAVE='true'";
                }
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();

                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch(SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        //This method was added to get all of the current saves for the user.
        //The saves are returned in a list delimited by a comma and then displayed
        //for the user.
        public List<string> GetSaves(int id)
        {
            try
            {
                List<string> saves = new List<string>();
                string query = "SELECT ID, SAVE_DATE_TIME, NAME FROM dbo.game WHERE USER_ID=@id";

                cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    saves.Add(dr.GetInt32(0) + "," + dr.GetDateTime(1) + "," + dr.GetString(2));
                }
                return saves;
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
        }
    }
}