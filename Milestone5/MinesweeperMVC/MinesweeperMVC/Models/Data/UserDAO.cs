/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      UserDAO.cs
 * @revision:  1.1
 * @synapsis:  This class registers the user and will complete any future user functions.
 *             Note that login is not a method in this class because the combo check 
 *             method already exists in the securityDAO class. It would be redundant to
 *             have a login function here as well.
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace MinesweeperMVC.Models.Data
{
    public class UserDAO
    {
        //Class variables
        private String connString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Minesweeper;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private String username, password;
        private bool res = false;
        SqlConnection conn;
        SqlDataReader dr;
        SqlCommand cmd;

        public UserDAO(UserModel model)
        {
            //Variables are given value from model in constructor
            username = model.Username;
            password = model.Password;
            conn = new SqlConnection(connString);
        }

        //This method is only called after security checks are completed
        public int Register(String hashPass)
        {
            Int32 newId = 0;
            try
            {
                String query = "INSERT INTO dbo.users (USERNAME, PASSWORD) OUTPUT INSERTED.ID VALUES(@Username, @Password)";
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@Username", SqlDbType.VarChar, 50).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = hashPass;

                conn.Open();

                newId = (Int32)cmd.ExecuteScalar();                
                conn.Close();
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }
            return newId;
        }

        //This method was added to get the user's id for the purpose of saving
        //games to the database.
        public int GetUserId()
        {
            int id = 0;
            try
            {
                string query = "SELECT * FROM dbo.users WHERE USERNAME=@username";
                cmd = new SqlCommand(query, conn);

                cmd.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;
                conn.Open();

                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    id = (int)dr["ID"];
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
                throw e;
            }

            return id;
        }
    }
}