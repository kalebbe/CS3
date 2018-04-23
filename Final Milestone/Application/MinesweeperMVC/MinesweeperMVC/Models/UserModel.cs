/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      UserModel.cs
 * @revision:  1.0
 * @synapsis:  The only purpose of this class currently is the model the user's data. I don't
 *             think anything will be added here in the future, but that may change.
 *             
 *             -----UPDATE FINAL-----
 *             -Added Data Annotations for form requirements and errors.
 */

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinesweeperMVC.Models
{
    public class UserModel
    {
        [Required]
        [DisplayName("Username")]
        [DefaultValue("")]
        public String Username { get; set; }

        [Required]
        [DisplayName("Password")]
        [StringLength(50, MinimumLength = 8)]
        [DefaultValue("")]
        public String Password { get; set; }

        [DisplayName("RePass")]
        [StringLength(50, MinimumLength = 8)]
        public String PassRe { get; set; }//We ask for pass twice in Reg
    }
}