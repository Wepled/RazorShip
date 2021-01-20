using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using DAL;
using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleBattleship
{
    class Program
    {
        private const int V = 1;
        public SeabBattleConsoleUI SeabBattleConsoleUI;

        public Program(SeabBattleConsoleUI seabBattleConsoleUi)
        {
            SeabBattleConsoleUI = seabBattleConsoleUi;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=========> SeaBattle <================");

            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("New game human vs AI. Pointless. U ll Lose)", "1", SeaBattle));

            menu.RunMenu();
        }

        static string DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");

            return "";
        }

        static string SeaBattle()
        {
            var dbOptions = new DbContextOptionsBuilder<BattleShipDbContext>().UseSqlServer(
                "Server = (localdb)\\mssqllocaldb; Database = BattleShipDbContext; Trusted_Connection = True; MultipleActiveResultSets = true"
            ).Options;

            using var dbCtx = new BattleShipDbContext(dbOptions);
            SeaBattle? game = new SeaBattle(dbCtx);
            Menu? menu = new Menu(MenuLevel.Level0);
            if (game != null)
            {
                menu.AddMenuItem(new MenuItem(
                $"Player Place a ship",
                userChoice: "p",
                () =>
                {
                    if (game.Game != null)
                    {
                        var (x, y) = GetMoveCordinates(game);
                        game.CreateShip(GetshipType(), true);
                        game.MoveShip(x, y, true, isRotate());
                        if (game.SelectedShip != null)
                        {
                            game.CreateFieldForBot(game.SelectedShip);
                            game.Save();
                            SeabBattleConsoleUI.DrawBoard(game.Game.Cells.Where(g => g.IsPlayer).ToList(), game);
                        }
                        else
                        {
                            Console.WriteLine("Do not working(");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Please Create Game or load");
                    }
                    return "";
                })
            );
                menu.AddMenuItem(new MenuItem(
                $"Random",
                userChoice: "random",
                () =>
                {
                    if (game.Game != null)
                    {
                        game.DoRandom(true);
                        if (game.SelectedShip != null)
                        {
                            game.CreateFieldForBot(game.SelectedShip);
                            game.Save();
                            SeabBattleConsoleUI.DrawBoard(game.Game.Cells.Where(g => g.IsPlayer).ToList(), game);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please Create Game or load");
                    }
                    return "";
                })
                );
                menu.AddMenuItem(new MenuItem(
                    $"Play game",
                    userChoice: "play",
                    () =>
                    {
                        if (game.Game != null)
                        {
                            return PlayBattle(game);
                        }
                        else
                        {
                            Console.WriteLine("Please Create Game or load");
                            return "";
                        }
                    })
                );

                menu.AddMenuItem(new MenuItem(
                    $"Create game with original settings",
                    userChoice: "s",
                    () =>
                    {
                        game.CreateGame();
                        return "";
                    })
                );
                menu.AddMenuItem(new MenuItem(
                    $"Load game",
                    userChoice: "l",
                    () =>
                    {
                        game = LoadGameAction(game);
                        return "";
                    })
                );
                menu.AddMenuItem(new MenuItem(
                    $"Create game with custom settings",
                    userChoice: "e",
                    () =>
                    {
                        Game gameToCreate = new Game();
                        gameToCreate.Game_Name = "CustomGame_" + game.GetGames().Count();
                        gameToCreate.Width = GetData("Width", V);
                        gameToCreate.Heigth = GetData("Heigth", V);
                        gameToCreate.BattleshipCount = GetData("Battleship", V);
                        gameToCreate.CruiserCount = GetData("Cruiser", V);
                        gameToCreate.SubmarineCount = GetData("Submarine", V);
                        gameToCreate.PatrolCount = GetData("Patrol", V);
                        gameToCreate.CarrierCount = GetData("Carrier", V);
                        gameToCreate.CanGoToAnother = IsShipsCanGo();
                        game.SaveGame(gameToCreate);
                        game.Game = gameToCreate;
                        game.Game.Cells = game.GenerateField();
                        Ship shipTocreate;

                        foreach (var shiptype in game.ShipsTypes)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                shipTocreate = game.CreateCustomShip(shiptype);
                                shipTocreate.Length = GetData(shiptype, 2);
                                shipTocreate.IsPlayer = i == V;
                                game.SaveShip(shipTocreate);
                            }
                        }
                        game.LoadGame(game.Game.Id);
                        return "";
                    })
                );

                var userChoice = menu.RunMenu();

                return userChoice;

            }
            return "";
        }

        static string PlayBattle(SeaBattle game)
        {
            if (game.Game != null)
            {
                SeabBattleConsoleUI.DrawBoard(game.Game.Cells.Where(g => g.IsPlayer).ToList(), game, true, true);
                SeabBattleConsoleUI.DrawBoard(game.Game.Cells.Where(g => !g.IsPlayer).ToList(), game, false, true);
                var menu = new Menu(MenuLevel.Level1);
                menu.AddMenuItem(new MenuItem(
                    $"Player MakeMove",
                    userChoice: "p",
                    () =>
                    {
                        var (x, y) = GetMoveCordinates(game);
                        game.MakeAMove(x, y);
                        game.MakeAMoveByBot();
                        game.Save();
                        SeabBattleConsoleUI.DrawBoard(game.Game.Cells.Where(g => g.IsPlayer).ToList(), game, true, true);
                        SeabBattleConsoleUI.DrawBoard(game.Game.Cells.Where(g => !g.IsPlayer).ToList(), game, false, true);
                        return "";
                    })
                );
                menu.RunMenu();
            }

            return "";
        }

        static (int x, int y) GetMoveCordinates(SeaBattle game)
        {
            if (game.Game != null)
            {
                Console.WriteLine("Upper left corner is (1,1)!");
                Console.Write("Give X (1-" + game.Game.Width + "), Y (1-" + game.Game.Heigth + "):");

                var userValue = Console.ReadLine().Split(',');
                var x = V;
                var y = V;
                try
                {
                    x = int.Parse(userValue[0].Trim());
                    y = int.Parse(userValue[V].Trim());
                }
                catch
                {
                    return GetMoveCordinates(game);
                }

                return (x, y);
            }
            return (0, 0);

        }

        static SeaBattle LoadGameAction(SeaBattle game)
        {
            Console.WriteLine("Games, Which to load");
            int i = 0;
            List<Game> games = game.GetGames();
            foreach (Game g in games)
            {
                i++;
                Console.WriteLine("[" + i + "] " + g.Game_Name);
            }
            game.LoadGame(games[int.Parse(Console.ReadLine()) - V].Id);
            return game;
        }
        static bool isRotate()
        {
            Console.WriteLine("isRotate, 1 if true 0 if false");
            return Console.ReadLine() == "1";
        }
        static string GetshipType()
        {
            Console.WriteLine("Ships, Which to place");
            List<string> ShipsTypes = new List<string>() { "Patrol", "Cruiser", "Submarine", "Battleship", "Carrier" };
            int i = 0;
            foreach (string type in ShipsTypes)
            {
                i++;
                Console.WriteLine("[" + i + "] " + type);
            }
            return ShipsTypes[int.Parse(Console.ReadLine()) - V];
        }
        static int GetData(string dataToHave, int mode)
        {
            if (mode == V)
            {
                Console.WriteLine(dataToHave + " of Game");
            }
            else
            {
                Console.WriteLine("How big is " + dataToHave);
            }

            return int.Parse(Console.ReadLine());
        }
        static bool IsShipsCanGo()
        {
            Console.WriteLine("IsShipsCanGo, 1 if true 0 if false");
            return Console.ReadLine() == "1";
        }
        static string SaveGameAction(SeaBattle game)
        {
            // 2020-10-12
            return "";
        }
    }
}
