/*
 * @author:    Kaleb Eberhart
 * @date:      04/20/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      IMineLogger.cs
 * @revision:  1
 * @synapsis:  This is the interface for the MineLogger. These methods must be used in the MineLogger.
 *             Not much to comment about here since this is just a contract.
 */

namespace MinesweeperMVC.Services.Utilities
{
    public interface IMineLogger
    {
        void Debug(string message, string arg = null);

        void Info(string message, string arg = null);

        void Warning(string message, string arg = null);

        void Error(string message, string arg = null);
    }
}