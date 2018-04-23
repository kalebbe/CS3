/*
 * @authors:   Kaleb Eberhart
 * @date:      02/11/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.5
 * @file:      GameLogic.cs
 * @revision:  1.1
 * @synapsis:  This business service contains all of the logic for our
 *             minesweeper game. The majority of this was taken from
 *             the cst-227 course code.
 */

using MinesweeperMVC.Controllers;
using MinesweeperMVC.Models;
using System;

namespace MinesweeperMVC.Services.Utilities
{
    public class GameLogic
    {
        private Random rnd = new Random();
        //TODO: Will be removed in favor of GameController.Size in next milestone.
        public static ButtonModel[,] BtnHolder;
        public static bool Lose = false;
        public static bool Win = false;
        public static int TurnCount;
        public static bool SaveGame = false;
        public int Size = GameController.Size; //Added out of laziness because I didn't want to change all of the Size to GameController.Size.

        //Method for generating the virtual board and its values.
        public void GenerateBoard()
        {
            BtnHolder = new ButtonModel[Size, Size];

            for(int i = 0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    BtnHolder[i, j] = new ButtonModel(i, j);

                    if (IsActive())
                    {
                        BtnHolder[i, j].IsLive = true;
                    }
                }
            }

            for(int i = 0; i < Size; i++)
            {
                for(int j=0; j < Size; j++)
                {
                    CountNeighbors(i, j);
                }
            }
        }

        //Gives each ButtonModel a 15% chance of being a bomb.
        public bool IsActive()
        {
            int rand = rnd.Next(1, 20);
            if (rand <= 3) return true;
            else return false; 
        }

        //Counts the amount of live neighbors surrounding each ButtonModel
        public void CountNeighbors(int x, int y)
        {
            int count = 0;
            if(!BtnHolder[x, y].IsLive)
            {
                //Nested loops to check each neighbor
                for(int i = -1; i <= 1; i++)
                {
                    for(int j = -1; j <= 1; j++)
                    {
                        if(x + i >= 0 && x + i < Size && y + j >= 0 && y + j < Size)
                        {
                            if (BtnHolder[x + i, y + j].IsLive) count++;
                        }
                    }
                }
                BtnHolder[x, y].LiveNeighbors = count;
            }
            else BtnHolder[x, y].LiveNeighbors = 9;
        }

        //This the meat of the logic here. This is called each time the user
        //clicks on a cell. Warning: recursion ahead.
        public static void ProcessCell(int x, int y)
        {
            if (CheckInbounds(x, y))
            {
                if (BtnHolder[x, y].IsLive == true)
                {
                    SetLose(GameController.Size);
                }

                else if (BtnHolder[x, y].LiveNeighbors != 0)
                {
                    BtnHolder[x, y].IsVisited = true;
                }
                else
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            //This checks to make sure all of the recursive calls are in bounds
                            if (CheckInbounds(x + i, y + j) && !BtnHolder[x, y].IsVisited)
                            {
                                //Recursive calls for each neighbor
                                BtnHolder[x, y].IsVisited = true;
                                ProcessCell(x - 1, y);
                                ProcessCell(x + 1, y);
                                ProcessCell(x, y - 1);
                                ProcessCell(x, y + 1);
                                ProcessCell(x + 1, y + 1);
                                ProcessCell(x - 1, y - 1);
                                ProcessCell(x + 1, y - 1);
                                ProcessCell(x - 1, y + 1);
                            }
                        }
                    }
                }
            }
        }

        //Very simple method that does as it says.
        public static bool CheckInbounds(int x, int y)
        {
            int max = GameController.Size;
            return x >= 0 && x < max && y >= 0 && y < max;

        }

        //This is needed because all the cells must be set to visited to display on a loss.
        public static void SetLose(int Size)
        {
            Lose = true;
            for (int i = 0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    BtnHolder[i, j].IsVisited = true;
                }
            }
        }

        //Quick and dirty algo for checking if the user has won.
        public static void CheckWin()
        {
            int count = 0;
            int bombCount = 0;
            int size = GameController.Size;
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if (BtnHolder[i, j].IsVisited)
                    {
                        count++;
                    }
                    if (BtnHolder[i, j].IsLive)
                    {
                        bombCount++;
                    }
                }
            }

            if(count == ((size * size) - bombCount))
            {
                Win = true;
            }
        }
    }
}