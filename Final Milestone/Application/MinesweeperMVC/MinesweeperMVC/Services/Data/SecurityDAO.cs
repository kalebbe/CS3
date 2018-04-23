/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      SecurityDAO.cs
 * @revision:  1.0
 * @synapsis:  This class handles all security checks that require database access.
 */
using MinesweeperMVC.Models;
using MinesweeperMVC.Services.Utilities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MinesweeperMVC.Services.Data
{
    public class SecurityDAO
    {
        private String connString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Minesweeper;Integrated Security=True;" +
            "Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private String user, pass, query;
        private SqlConnection cn;
        private SqlCommand cmd;
        private SqlDataReader dr;
        private bool res;

        public SecurityDAO(UserModel model)
        {
            MineLogger.getInstance().Info("SecurityDAO instantiated");
            user = model.Username;
            pass = model.Password;
            cn = new SqlConnection(connString);
        }

        public String GetPass ()
        {
            MineLogger.getInstance().Info("GetPass method invoked in SecurityDAO");
            string results = "";
            try
            {
                //We do not use WHERE PASSWORD=@ here because we can't check plaintext pass with hashed
                //so we must retrieve the password and then decipher it before checking for the match.
                query = "SELECT * FROM dbo.Users WHERE USERNAME=@username";
                cmd = new SqlCommand(query, cn);

                cmd.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = user;

                cn.Open();

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    results = dr["PASSWORD"].ToString(); //Saving the hashed password to send out
                }
                return results;
            }
            catch(SqlException e)
            {
                MineLogger.getInstance().Error("Exception caught: " + e.ToString());
                throw e;
            }
        }

        //This method is just for checking if the username exists
        public bool CheckExisting()
        {
            MineLogger.getInstance().Info("CheckExisting method invoked in SecurityDAO");
            try
            {
                query = "SELECT * FROM dbo.users WHERE USERNAME=@username";
                cmd = new SqlCommand(query, cn);

                cmd.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = user;

                cn.Open();

                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    MineLogger.getInstance().Warning("User tried to register existing username");
                    res = false;
                }
                else
                {
                    res = true;
                }
                return res;
            }
            catch(SqlException e)
            {
                MineLogger.getInstance().Error("Exception caught: " + e.ToString());
                throw e;
            }
        }
    }
}