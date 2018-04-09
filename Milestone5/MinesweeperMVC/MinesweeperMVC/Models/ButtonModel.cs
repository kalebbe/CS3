/*
 * @authors:   Kaleb Eberhart
 * @date:      02/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      ButtonModel.cs
 * @revision:  1.1
 * @synapsis:  This model holds the information for each
 *             button in the multidimensional array. These
 *             properties are all self explanatory, so I won't
 *             be adding any comments.
 */

namespace MinesweeperMVC.Models
{
    public class ButtonModel
    {
        public bool IsLive { get; set; }
        public int LiveNeighbors { get; set; }
        public bool IsVisited { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Flagged { get; set; }
        
        public ButtonModel(int x, int y)
        {
            this.IsLive = false;
            this.IsVisited = false;
            this.LiveNeighbors = 0;
            this.X = x;
            this.Y = y;
            this.Flagged = false;
        }

        //For debugging purposes only.
        public override string ToString()
        {
            return "(" + X + "," + Y + ") " + "L?=" + IsLive + ". LN=" + LiveNeighbors + ". VIS?=" + IsVisited + ". Flag?=" + Flagged;
        }
    }
}