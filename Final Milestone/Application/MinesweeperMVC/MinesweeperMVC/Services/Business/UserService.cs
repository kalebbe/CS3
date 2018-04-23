/*
 * @authors:   Kaleb Eberhart
 * @date:      04/22/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      UserService.cs
 * @revision:  1.0
 * @synapsis:  This class was added to incorporate layered architecture in my
 *             project. The controllers will no longer call the DAOs directly, but
 *             will go through this service instead.
 */

using MinesweeperMVC.Models;
using MinesweeperMVC.Services.Data;
using MinesweeperMVC.Services.Utilities;

namespace MinesweeperMVC.Services.Business
{
    public class UserService
    {
        private UserDAO ud;
        private SecurityDAO sd;
        private StatsDAO std;
        private GameDAO gd;
        private SecurityService ss;

        //Constructor instantiates all DAOs used in this class.
        public UserService(UserModel model)
        {
            MineLogger.getInstance().Info("UserService instantiated");
            ud = new UserDAO(model);
            sd = new SecurityDAO(model);
            std = new StatsDAO();
            gd = new GameDAO();
            ss = new SecurityService(model);
        }

        //Calls UserDAO to register user and create a gamestats slot for them.
        public void Register(string pass)
        {
            MineLogger.getInstance().Info("Register method invoked in UserService");
            int id = ud.Register(pass); //This method returns the id of the newly registered user.
            std.CreatePlayer(id); //This creates a gamestats slot with the new id.
        }

        //Calls SecurityDAO to check if the username is existent.
        public bool CheckExisting()
        {
            MineLogger.getInstance().Info("CheckExisting method invoked in UserService");
            if (sd.CheckExisting()) return true;
            
            else return false;
        }

        //Calls SecurityDAO to get the user's hashed password then checks the 
        //entered password with the database password to login the user.
        public bool Login()
        {
            MineLogger.getInstance().Info("Login method invoked in UserService");
            string hashPass = sd.GetPass();
            if (ss.CheckHash(hashPass))
            {
                return true;
            }
            else return false;
        }

        //Calls UserDAO to get the user's id
        public int GetUserId()
        {
            MineLogger.getInstance().Info("GetUserId method invoked in UserService");
            return ud.GetUserId();
        }

        //Calls GameDAO to check if the user has saves.
        public bool CheckSaves(int id, bool auto)
        {
            MineLogger.getInstance().Info("CheckSaves method invoked in UserService");
            return gd.CheckSaves(id, auto);
        }
    }
}