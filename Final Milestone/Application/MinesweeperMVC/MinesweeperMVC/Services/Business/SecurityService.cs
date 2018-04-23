/*
 * @authors:   Kaleb Eberhart
 * @date:      01/26/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      SecurityService.cs
 * @revision:  1.0
 * @synapsis:  This business service is used to complete all security checks
 *             that do not require access to the database.
 */

using MinesweeperMVC.Models;
using MinesweeperMVC.Services.Utilities;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MinesweeperMVC.Services.Business
{
    public class SecurityService
    {
        private String username, password, rePass;
        private bool res;

        //Constructor sets class variables to model traits.
        public SecurityService(UserModel user)
        {
            MineLogger.getInstance().Info("SecurityService instantiated");
            username = user.Username;
            password = user.Password;
            rePass = user.PassRe;
        }

        //Checks password with a regex to ensure password meets requirements.
        public bool ValidatePassword()
        {
            MineLogger.getInstance().Info("ValidatePassword method invoked in SecurityService");
            var num = new Regex(@"[0-9]+"); //Regex to check if input has numbers
            var letters = new Regex(@"[a-zA-Z]+"); //Regex to check for lower or upper case letters

            if (num.IsMatch(password) && letters.IsMatch(password) && password.Equals(rePass))
            {
                return true;
            }
            else
            {
                MineLogger.getInstance().Warning("User tried to register with invalid password");
                return false;
            }
        }

        //Resource:
        //  https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        //This page was used to create the algorithm for encrypting and decrypting passwords.
        //It's a much more complicated hashing algorithm than I'm used to dealing with.
        //This method is used to salt and hash passwords before storing them in the db.
        public String PasswordHasher()
        {
            MineLogger.getInstance().Info("PasswordHasher method invoked in SecurityService");

            //This is the salt being created here (obv)
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            //This creates the key and gets the hash value. 10000 is a bit excessive
            //for this small of a project, but why not.
            var derBy = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = derBy.GetBytes(20);

            //This combines the salt and hash to create your hard to decrypt
            //password.
            byte[] hashBytes = new Byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //Converts the encryption into a string for database storage.
            String compHash = Convert.ToBase64String(hashBytes);
            return compHash;
        }

        //This method was occassionally failing because of some problem
        //with array size.. Since adding the try/catch block, I have not
        //been able to recreate the error. If it continues, we will probly
        //downgrade to md5 hashing. This uses the same resource as the
        //previous method.
        public bool CheckHash(String hPass)
        {
            MineLogger.getInstance().Info("CheckHash method invoked in SecurityService");
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hPass); //converting from string to byte

                //Creating the salt, same as before
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                //Derives the key with the salt and password 10k times to decrypt the password
                var derBy = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = derBy.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        MineLogger.getInstance().Warning("User's password did not match stored password");
                        res = false;
                    }
                    else
                    {
                        res = true;
                    }
                }
                return res;
            }
            catch(Exception e) //TODO: feedback to user
            {
                MineLogger.getInstance().Error("Exception caught: " + e.ToString());
                throw e;
            }
        }
    }
}