using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using GameBrain;

namespace ConsoleBattleship
{
    public class SeabBattleConsoleUI
    {
        public static void DrawBoard(List<Cell> cells, SeaBattle game, bool IsPlayer = true, bool isPlayMode = false)
        {
            if (game.Game != null) 
            {
                List<char> Letters = new List<char>() { '0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
                string log = "";
                Doline(game.Game.Width + 1);
                for (int i = 0; i < game.Game.Heigth + 1; i++)
                {
                    log += "|" + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                    for (int j = 0; j < game.Game.Width + 1; j++)
                    {
                        if (i > 0 && j > 0)
                        {
                            if (isPlayMode)
                            {
                                if (cells.Single(c => c.X == j && c.Y == i).ShipName != null && cells.Single(c => c.X == j && c.Y == i).IsHit)
                                {
                                    log += "XS" + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + "|" +
                                           String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                                }
                                else if (cells.Single(c => c.X == j && c.Y == i).ShipName == null && cells.Single(c => c.X == j && c.Y == i).IsHit)
                                {
                                    log += " X" + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + "|" +
                                           String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                                }
                                else if (cells.Single(c => c.X == j && c.Y == i).ShipName != null && !cells.Single(c => c.X == j && c.Y == i).IsHit && IsPlayer)
                                {

                                    log += " S" + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + "|" +
                                           String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                                }
                                else
                                {
                                    log += "  " + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + "|" +
                                           String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                                }
                            }
                            else
                            {
                                if (cells.Single(c => c.X == j && c.Y == i).ShipName != null && IsPlayer)
                                {
                                    log += " S" + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + "|" +
                                           String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                                }
                                else
                                {
                                    log += "  " + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + "|" +
                                           String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                                }
                            }

                        }

                        if (i == 0 && j == 0)
                        {
                            log += " " + String.Concat(Enumerable.Repeat(" ", 111.ToString().Length)) + "|";
                        }
                        else if (i == 0 && j.ToString().Length == 2)
                        {
                            log += String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + j + String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + "|";
                        }
                        else if (i == 0)
                        {
                            log += String.Concat(Enumerable.Repeat(" ", 11.ToString().Length)) + j + String.Concat(Enumerable.Repeat(" ", 111.ToString().Length)) + "|";
                        }
                        else if (j == 0)
                        {
                            log += Letters[i] + String.Concat(Enumerable.Repeat(" ", 111.ToString().Length)) + "|" +
                                   String.Concat(Enumerable.Repeat(" ", 11.ToString().Length));
                        }
                    }

                    Console.WriteLine(log);
                    log = "";
                }
                Doline(game.Game.Width + 1);
            }
            
        }

        public static void Doline(int max_value)
        {
            for (int colIndex = 0; colIndex < max_value; colIndex++)
            {
                if (colIndex == max_value - 1)
                {
                    Console.Write($"+------+");
                }
                else
                {
                    Console.Write($"+------");
                }
            }
            Console.WriteLine();
        }
    }
}