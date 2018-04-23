/*
 * @author:    Kaleb Eberhart
 * @date:      04/20/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      MineLogger.cs
 * @revision:  1
 * @synapsis:  This class sets up the custom logger for my project.
 */

using NLog;

namespace MinesweeperMVC.Services.Utilities
{
    public class MineLogger : IMineLogger
    {
        private static MineLogger _instance;

        //This method allows me to create an instance and call it a method in 1 line.
        public static MineLogger getInstance()
        {
            return _instance ?? (_instance = new MineLogger());
        }

        //This method gets the logger for all of the other methods using the config file.
        public static Logger getLogger()
        {
            return LogManager.GetLogger("myLoggerRules");
        }

        //Logs a Debug message for the developer (me).
        public void Debug(string message, string arg = null)
        {
            if (arg == null)
                getLogger().Debug(message);
            else
                getLogger().Debug(message, arg);
        }

        //Logs information for the developer
        public void Info(string message, string arg = null)
        {
            if (arg == null)
                getLogger().Info(message);
            else
                getLogger().Info(message, arg);
        }

        //Logs a warning for the developer
        public void Warning(string message, string arg = null)
        {
            if (arg == null)
                getLogger().Warn(message);
            else
                getLogger().Warn(message, arg);
        }

        //Logs an error for the developer
        public void Error(string message, string arg = null)
        {
            if (arg == null)
                getLogger().Error(message);
            else
                getLogger().Error(message, arg);
        }
    }
}