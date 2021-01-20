using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConsoleBattleship
{
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2Plus
    }

    public class Menu
    {
        // list is not really optimal choice
        //private List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        private Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();

        private readonly MenuLevel _menuLevel;

        private readonly string[] reservedActions = new[] {"x", "m", "r"};

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }


        public void AddMenuItem(MenuItem item)
        {
            if (item.UserChoice == "")
            {
                throw new ArgumentException($"UserChoice cannot be empty");
            }

            MenuItems.Add(item.UserChoice, item);
        }

        public string RunMenu() // need to be of type Func<string> 
        {
            var userChoice = "";
            do
            {
                Console.Write("");

                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem.Value);
                }

                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine("X) eXit");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) eXit");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine("R) Return to pervious");
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) eXit");
                        break;
                    default:
                        throw new Exception("Unknown menu depth!");
                }

                Console.Write(">");

                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";

                // is it a reserved keyword
                if (!reservedActions.Contains(userChoice))
                {
                    // no it wasn't, try to find keyword in MenuItems
                    if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                    {
                        userChoice = userMenuItem.MethodToExecute();
                    }
                    else
                    {
                        Console.WriteLine("I don't have this option!");
                    }
                }

                if (userChoice == "x")
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.WriteLine("Closing down......");
                    }

                    break;
                }

                if (_menuLevel != MenuLevel.Level0 && userChoice == "m")
                {
                    break;
                }

                if (_menuLevel == MenuLevel.Level2Plus && userChoice == "r")
                {
                    break;
                }
            } while (true);

            return userChoice;
        }
    }

    public class MenuItem
    {
        public virtual string Label { get; set; }
        public virtual string UserChoice { get; set; }

        public virtual Func<string> MethodToExecute { get; set; }

        public MenuItem(string label, string userChoice, Func<string> methodToExecute)
        {
            Label = label.Trim();
            UserChoice = userChoice.Trim();
            MethodToExecute = methodToExecute;
        }

        public override string ToString()
        {
            return UserChoice + ") " + Label;
        }

    }
}