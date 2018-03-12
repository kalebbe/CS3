/*
 * @authors:   Kaleb Eberhart
 * @date:      03/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.3
 * @file:      GameDAO.cs
 * @revision:  1.0
 * @synapsis:  This class is used to save the user's game, delete it, and retrieve it. Will
 *             be updated with the next milestone to allow multiple (custom) saves.
 */

using MinesweeperMVC.Controllers;
using MinesweeperMVC.Models.Business;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;

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

        public bool SaveGame(int userId, ButtonModel[,] bm)
        {
            try
            {
                string query;

                //Checks to see if the user already has a saved game and either inserts or updates depending on which.
                if (CheckExistingGame(userId))
                {
                    query = "UPDATE dbo.game SET GAME_BOARD=@game, SAVE_DATE_TIME=@dt, SIZE=@size WHERE USER_ID=@id";
                }
                else
                {
                    query = "INSERT INTO dbo.game (USER_ID, GAME_BOARD, SAVE_DATE_TIME, SIZE) VALUES(@id, @game, @dt, @size)";
                }
                conn.Open();

                //This converter allows me to serialize and deserialize Multidimensional arrays. Huge life saver after trying the script serializer.
                string json = JsonConvert.SerializeObject(bm, Formatting.Indented);

                cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@game", SqlDbType.VarChar).Value = json;
                cmd.Parameters.Add("@dt", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@size", SqlDbType.Int).Value = GameController.Size;

                //Kind of wish this returned a bool, so I could just return cmd.ExecuteNonQuery like in php >.>
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

        //This method was added to check if the user has a saved game already to help with saving.
        //This method will need to be adapted with the next milestone to check multiple saves and whatnot.
        public bool CheckExistingGame(int id)
        {
            try
            {
                string query = "SELECT * FROM dbo.game WHERE USER_ID=@id";

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
                string query = "SELECT * FROM dbo.game WHERE USER_ID=@id";

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
        public void DeleteSave(int id)
        {
            try
            {
                string query = "DELETE FROM dbo.game WHERE USER_ID=@id";

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
    }
}