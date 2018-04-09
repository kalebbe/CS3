/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      UserModel.cs
 * @revision:  1.0
 * @synapsis:  The only purpose of this class currently is the model the user's data. I don't
 *             think anything will be added here in the future, but that may change.
 */

using System;

namespace MinesweeperMVC.Models
{
    public class UserModel
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public String PassRe { get; set; }//We ask for pass twice in Reg
    }
}