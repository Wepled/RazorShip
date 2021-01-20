using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain;
using DAL;
using GameBrain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace WebBattleship.Pages
{
    public class OriginalSettingsGame : PageModel
    {
        public List<char> Letters = new List<char>() { '0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        public List<string> ShipsTypes = new List<string>() { "Patrol", "Cruiser", "Submarine", "Battleship", "Carrier" };

        public ShipGameAssignment? SelectedShip;

        private readonly BattleShipDbContext _context;
        public SeaBattle GameBrain;
        public Game? Game;

        public OriginalSettingsGame(BattleShipDbContext context)
        {
            _context = context;
            GameBrain = new SeaBattle(_context);
        }
        public void OnGet(int? GameId,
            string? ShipId,
            string? settings,
            int? x, int? y,
            bool? isNewShip,
            string? shipType,
            int? random,
            bool? isRotate)
        {

            if (GameId == null && settings == "original")
            {
                GameBrain.CreateGame();
                GameBrain.CreateShips();
                GameBrain.GenerateField();

            }
            else
            {
                if (GameId != null)
                {
                    GameBrain.LoadGame(GameId.Value);
                }
            }
            GameBrain.GenerateField();
            GameBrain.ReCalcCounters();

            if (isNewShip == true && shipType != null)
            {
                GameBrain.CreateShip(shipType, true);
                GameBrain.Save();
            }

            if (ShipId != null)
            {
                if (isNewShip != true)
                {
                    GameBrain.SelectedShip = _context.ShipGameAssignments.Single(x => x.ShipName == ShipId && x.IsPlayer);
                    if (GameBrain.Game != null)
                    {
                        GameBrain.SelectedShip.Cells = GameBrain.Game.Cells.Where(x => x.IsPlayer && x.ShipName == GameBrain.SelectedShip.ShipName).ToList();
                    }
                }

                if (GameBrain.SelectedShip != null && GameBrain.Game != null)
                {
                    GameBrain.SelectedShip.Game_Id = GameBrain.Game.Id;
                    if (isRotate == true)
                    {
                        GameBrain.MoveShip(x, y, true, true);
                        GameBrain.SelectedShip.IsRotated = !GameBrain.SelectedShip.IsRotated;
                    }
                    else
                    {
                        GameBrain.MoveShip(x, y, true);
                    }

                }

            }


            if (random == 1)
            {
                GameBrain.DoRandom(true);
            }

            if (GameBrain.SelectedShip != null)
            {
                GameBrain.CreateFieldForBot(GameBrain.SelectedShip);
            }

            Game = GameBrain.Game;
            SelectedShip = GameBrain.SelectedShip;

            _context.SaveChanges();
        }

    }
}