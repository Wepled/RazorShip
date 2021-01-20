using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebBattleship.Pages
{
    public class Start : PageModel
    {
        public List<char> Letters = new List<char>() { '0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        public Game? Game;
        public List<Cell>? Cells;
        public bool IsBotWinner = false;
        public bool IsPlayerWinner = false;
        private readonly BattleShipDbContext _context;

        public Start(BattleShipDbContext context)
        {
            _context = context;
        }
        public void OnGet(int gameId, int? x, int? y)
        {
            Game = _context.Games.Single(x => x.Id == gameId);
            Game.Cells = _context.Cells.Where(x => x.GameId == gameId).ToList();

            IsBotWinner = Game.Cells.Where(c => c.ShipName != null && c.IsPlayer).Count() ==
                          Game.Cells.Where(c => c.ShipName != null && c.IsPlayer && c.IsHit).Count();
            IsPlayerWinner = Game.Cells.Where(c => c.ShipName != null && !c.IsPlayer).Count() ==
                             Game.Cells.Where(c => c.ShipName != null && !c.IsPlayer && c.IsHit).Count();

            if (!IsBotWinner && !IsPlayerWinner)
            {
                MakeAMove(x, y);
                MakeAMoveByBot();
                _context.SaveChanges();
            }

        }

        public void MakeAMoveByBot()
        {
            if (Game != null) 
            {
                Random rnd = new Random();
                List<Cell> botCells = _context.Cells.Where(c => c.IsPlayer && c.IsHit == false && c.GameId == Game.Id).ToList();
                Cell cell = botCells[rnd.Next(botCells.Count())];
                MakeAMove(cell.X, cell.Y, true);
            }
          
        }

        public void MakeAMove(int? x, int? y, bool isplayer = false)
        {
            if (x != null && y != null && Game != null)
            {
                Game.Cells.Single(c => c.X == x && c.Y == y && c.IsPlayer == isplayer && c.GameId == Game.Id).IsHit = true;
                Cell shipCell = _context.Cells.Single(c => c.X == x && c.Y == y && c.IsPlayer == isplayer && c.GameId == Game.Id);
                if (shipCell.ShipName != null && Game.CanGoToAnother) 
                {
                    ShipGameAssignment shipToKill = _context.ShipGameAssignments.Single(s => s.ShipName == shipCell.ShipName);
                    shipToKill.Cells = Game.Cells.Where(c => c.ShipName == shipToKill.ShipName).ToList();
                    if (ShipIsKilled(Game.Cells.Single(c => c.X == x && c.Y == y && c.IsPlayer == isplayer && c.GameId == Game.Id).ShipName))
                    {
                        SetKillShip(shipToKill);
                    }
                }
            }
        }

        public string GetCellClass(int x, int y, bool isPlayer)
        {
            if (Game != null)
            {
                if (y == 0 || x == 0)
                {
                    return "";
                }
                Cell cell = Game.Cells.Single(c => c.CellId == Letters[y].ToString() + x && c.IsPlayer == isPlayer);
                if (ShipIsKilled(cell.ShipName))
                {
                    return "bg-dark";
                }
                if (cell.IsHit && cell.ShipName != null)
                {
                    return "bg-primary";
                }
                if (cell.IsHit)
                {
                    return "bg-warning";
                }

                return "";
            }
            return "";

        }
        public void SetKillShip(ShipGameAssignment ship)
        {
            if (Game != null && ship.Cells != null)
            {
                int x;
                int y;
                foreach (Cell cell in ship.Cells)
                {
                    x = cell.X;
                    y = cell.Y;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (x + j > 0 && x + j < Game.Width + 1 && y + i > 0 && y + i < Game.Heigth + 1)
                            {
                                Cell cell_to_hit = Game.Cells.Single(c => c.X == x + j && c.Y == y + i && c.IsPlayer == ship.IsPlayer);
                                cell_to_hit.IsHit = true;
                            }

                        }
                    }
                }
            }
        }

        public bool ShipIsKilled(string? shipName)
        {
            if (Game != null) 
            {
                foreach (Cell cellT in Game.Cells.Where(c => c.ShipName == shipName))
                {
                    if (!cellT.IsHit)
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
        public bool CheckCell(int x, int y, bool isPlayer)
        {
            if (Game != null) 
            {
                List<Cell> finList = _context.Cells.Where(c => c.GameId == Game.Id && c.IsPlayer == isPlayer).ToList();
                finList = finList.Where(c => c.X == x).ToList();
                return finList.Single(c => c.Y == y).ShipName != null;
            }
            return false;
            
        }
    }
}