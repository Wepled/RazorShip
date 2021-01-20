using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using DAL;
using Domain;


namespace GameBrain
{
    public class SeaBattle
    {
        public List<char> Letters = new List<char>() { '0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        public char GetLetter(int i) => (i > 0 && i < 27) ? (char)(64 + i) : throw new ArgumentOutOfRangeException(i.ToString());
        public readonly string[] ShipsTypes = new[]{ "Patrol", "Cruiser", "Submarine", "Battleship", "Carrier" };

        public Game? Game { get; set; }
        public ShipGameAssignment? SelectedShip { get; set; }
        private readonly BattleShipDbContext _context;

        public int CarrierCount = 0;
        public int BattleshipCount = 0;
        public int SubmarineCount = 0;
        public int CruiserCount = 0;
        public int PatrolCount = 0;

        public SeaBattle(BattleShipDbContext context)
        {
            _context = context;
        }

        public void CreateGame()
        {
            Game game = new Game();
            game.Heigth = 10;
            game.Width = 10;
            game.Game_Name = "Game" + _context.Games.Count();
            game.BattleshipCount = 1;
            game.CarrierCount = 1;
            game.SubmarineCount = 1;
            game.PatrolCount = 1;
            game.CruiserCount = 1;
            game.IsPlayerMove = true;
            _context.Games?.Add(game);
            _context.SaveChanges();
            Game = game;
            CreateShips();
            game.Cells = GenerateField();

        }
        public void LoadGame(int id)
        {
            Game = _context.Games.Single(x => x.Id == id);
            Game.Cells = _context.Cells.Where(x => x.GameId == id).ToList();
            if (Game.Cells.Count == 0) 
            {
                Game.Cells = GenerateField();
            }
            Game.Ships = _context.Ships.Where(s => s.GameId == Game.Id).ToList();
        }

        public void CreateShips()
        {
            if (Game != null) 
            {
                List<Ship> Ships = new List<Ship>();
                for (int i = 0; i < ShipsTypes.Length; i++)
                {
                    Ship ship = new Ship();
                    ship.GameId = Game.Id;
                    ship.Length = i + 1;
                    ship.Type = ShipsTypes[i];
                    ship.IsPlayer = true;
                    Ships.Add(ship);
                }

                for (int i = 0; i < ShipsTypes.Length; i++)
                {
                    Ship ship = new Ship();
                    ship.GameId = Game.Id;
                    ship.Length = i + 1;
                    ship.Type = ShipsTypes[i];
                    ship.IsPlayer = false;
                    Ships.Add(ship);
                }
                _context.Ships?.AddRange(Ships);
                _context.SaveChanges();
                Game.Ships = Ships;
            }
        }

        public void MoveShip(int? x, int? y, bool isPlayer, bool isRotate = false)
        {
            if (SelectedShip != null)
            {
                int length = SelectedShip.Length;
                int old_x = 0;
                int old_y = 0;

                if (SelectedShip.Cells != null)
                {
                    Cell downCell = GetDownCell(SelectedShip.Cells);
                    old_x = downCell.X;
                    old_y = downCell.Y;
                }

                if (x == null)
                {
                    x = 0;
                }

                if (y == null)
                {
                    y = 0;
                }


                if (IsShipCanGo(SelectedShip.ShipName, length, old_x + x.Value, old_y + y.Value, isPlayer, isRotate))
                {

                    if (SelectedShip.Cells != null)
                    {
                        Clear_Fields(SelectedShip.Cells);
                    }

                    string cellId = "";
                    SelectedShip.Cells = new List<Cell>();
                    for (int i = 0; i < length; i++)
                    {
                        if (SelectedShip.IsRotated != isRotate)
                        {
                            cellId = Letters[old_y + y.Value] + (old_x + x.Value + i).ToString();
                        }
                        else
                        {
                            cellId = Letters[old_y + y.Value - i] + (old_x + x.Value).ToString();
                        }
                        if (Game != null)
                        {
                            Cell shipCell = Game.Cells.Single(x => x.CellId == cellId && x.GameId == Game.Id && x.IsPlayer == isPlayer);
                            shipCell.ShipName = SelectedShip.ShipName;
                            SelectedShip.Cells.Insert(i, shipCell);
                        }
                    }
                }

            }
        }
        public List<Cell> GenerateField()
        {
            if (Game != null)
            {
                if (Game.Cells == null || Game.Cells.Count() == 0)
                {
                    bool isPlayer = true;
                    List<Cell> cells = new List<Cell>();
                    for (int k = 0; k < 2; k++)
                    {
                        if (k == 1)
                        {
                            isPlayer = false;
                        }
                        for (int i = 1; i < Game.Heigth + 1; i++)
                        {
                            for (int j = 1; j < Game.Width + 1; j++)
                            {
                                Cell cell = new Cell();
                                cell.GameId = Game.Id;
                                cell.X = j;
                                cell.Y = i;
                                cell.CellId = Letters[i] + j.ToString();
                                cell.IsPlayer = isPlayer;
                                cells.Add(cell);
                            }
                        }
                    }

                    return cells;
                }
                else
                {
                    return Game.Cells;
                }
            }
            return new List<Cell>();
        }

        public void Clear_Fields(List<Cell> cells)
        {
            foreach (Cell cell in cells)
            {
                cell.ShipName = null;
            }
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

        public void ReCalcCounters()
        {
            if (Game != null)
            {
                List<ShipGameAssignment> finList = _context.ShipGameAssignments.Where(c => c.Game_Id == Game.Id)
                .ToList();
                CarrierCount = finList.Where(c => c.Type == "Carrier").Count();
                BattleshipCount = finList.Where(c => c.Type == "Battleship").Count();
                SubmarineCount = finList.Where(c => c.Type == "Submarine").Count();
                CruiserCount = finList.Where(c => c.Type == "Cruiser").Count();
                PatrolCount = finList.Where(c => c.Type == "Patrol").Count();
            }

        }

        public Cell GetDownCell(List<Cell>? cells)
        {
            if (Game != null && SelectedShip != null)
            {
                if (cells == null)
                {
                    Cell newCell = new Cell();
                    newCell.GameId = Game.Id;
                    newCell.Y = 0;
                    newCell.X = 0;
                    newCell.IsHit = false;
                    return newCell;
                }

                if (SelectedShip.IsRotated)
                {
                    cells = cells.OrderBy(i => i.X).ToList();
                }
                else
                {
                    cells = cells.OrderByDescending(i => i.Y).ToList();
                }
                return cells[0];
            }
            return new Cell();
        }

        public void CreateShip(string type, bool isPlayer)
        {
            if (Game != null)
            {
                Ship shipTypoToCreate = _context.Ships.Single(x => x.Type == type && x.GameId == Game.Id && x.IsPlayer == isPlayer);
                string shipName;
                if (CheckCounters(shipTypoToCreate.ShipCounter, type))
                {
                    shipTypoToCreate.ShipCounter++;
                    shipName = shipTypoToCreate.Type + "_" + Game.Id + "_" + shipTypoToCreate.ShipCounter + "_" + (isPlayer ? "Player" : "Bot");
                    SelectedShip = new ShipGameAssignment();
                    SelectedShip.Game_Id = Game.Id;
                    SelectedShip.Length = shipTypoToCreate.Length;
                    SelectedShip.Type = shipTypoToCreate.Type;
                    SelectedShip.IsPlayer = isPlayer;
                    SelectedShip.IsRotated = false;
                    SelectedShip.ShipName = shipName;
                    _context.ShipGameAssignments?.Add(SelectedShip);
                }
            }
        }

        public bool IsShipCanGo(string? shipName, int length, int new_x, int new_y, bool isPlayer, bool isRotate = false)
        {
            if (Game != null)
            {
                Cell cellToInvestigate = new Cell();
                List<Cell> cellsToInvestigate = Game.Cells.Where(c => c.IsPlayer == isPlayer).ToList();
                for (int i = 0; i < length; i++)
                {

                    try
                    {
                        if (isRotate)
                        {
                            cellToInvestigate = cellsToInvestigate.Single(c => c.X == new_x + i && c.Y == new_y && c.IsPlayer == isPlayer && c.GameId == Game.Id);
                        }
                        else
                        {
                            cellToInvestigate = cellsToInvestigate.Single(c => c.X == new_x && c.Y == new_y - i && c.IsPlayer == isPlayer && c.GameId == Game.Id);
                        }

                        if (cellToInvestigate.ShipName != null && cellToInvestigate.ShipName != shipName)
                        {
                            return false;
                        }

                        if (!Game.CanGoToAnother && cellsToInvestigate != null)
                        {
                            List<Cell> CheckedCells = cellsToInvestigate.Where(c => c.ShipName != null ).ToList();
                             
                            foreach (Cell cell in CheckedCells)
                            {
                                if (Math.Sqrt(Math.Pow(cellToInvestigate.X - cell.X, 2) + Math.Pow(cellToInvestigate.Y - cell.Y, 2)) <= Math.Sqrt(2) && cell.ShipName != shipName)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }

                return true;
            }
            return false;
        }

        public bool CheckCounters(int counter, string type)
        {
            if (Game != null)
            {
                switch (type)
                {
                    case "Carrier":
                        if (Game.CarrierCount == counter)
                        {
                            return false;
                        }
                        break;
                    case "Battleship":
                        if (Game.BattleshipCount == counter)
                        {
                            return false;
                        }
                        break;
                    case "Submarine":
                        if (Game.SubmarineCount == counter)
                        {
                            return false;
                        }
                        break;
                    case "Cruiser":
                        if (Game.CruiserCount == counter)
                        {
                            return false;
                        }
                        break;
                    case "Patrol":
                        if (Game.PatrolCount == counter)
                        {
                            return false;
                        }
                        break;
                }
                return true;
            }
            return false;
        }

        public int GetShipNumber(string type)
        {
            if (Game != null)
            {
                return _context.Ships.Single(s => s.GameId == Game.Id && s.IsPlayer && s.Type == type).Length; ;
            }
            return 0;
        }


        public void DoRandom(bool isPlayer)
        {
            if (Game != null)
            {
                Clear_Fields(Game.Cells.Where(c => c.GameId == Game.Id && c.IsPlayer == isPlayer).ToList());
                ClearCounts(isPlayer);
                _context.ShipGameAssignments?.RemoveRange(_context.ShipGameAssignments.Where(s => s.Game_Id == Game.Id && s.IsPlayer == isPlayer));
                bool iscreated;
                bool isRotated;
                Random rnd = new Random();
                int x = 0;
                int y = 0;
                foreach (Ship ship in _context.Ships.Where(s => s.GameId == Game.Id && s.IsPlayer == isPlayer))
                {
                    iscreated = false;
                    for (int i = 0; i < ship.Length; i++)
                    {
                        if (ship.Type != null)
                        {
                            CreateShip(ship.Type, isPlayer);
                            if (SelectedShip != null)
                            {
                                while (!iscreated)
                                {
                                    isRotated = rnd.Next(2) == 1 ? true : false;
                                    x = rnd.Next(Game.Width);
                                    y = rnd.Next(Game.Heigth);
                                    if (IsShipCanGo(SelectedShip.ShipName, SelectedShip.Length, x, y, isPlayer, isRotated))
                                    {
                                        MoveShip(x, y, isPlayer, isRotated);
                                        iscreated = true;
                                    }
                                }
                                iscreated = false;
                            }
                        }
                    }
                }
            }
        }

        public void ClearCounts(bool isPlayer)
        {
            if (Game != null)
            {
                List<Ship> ships = _context.Ships.Where(s => s.GameId == Game.Id && s.IsPlayer == isPlayer).ToList();
                foreach (Ship ship in ships)
                {
                    ship.ShipCounter = 0;
                }
            }
        }

        public void CreateFieldForBot(ShipGameAssignment ship)
        {
            DoRandom(false);
            SelectedShip = ship;
        }
        public void MakeAMove(int? x, int? y, bool isplayer = false)
        {
            if (x != null && y != null && Game != null)
            {
                Game.Cells.Single(c => c.X == x && c.Y == y && c.IsPlayer == isplayer && c.GameId == Game.Id).IsHit = true;
            }
        }

        public List<Game> GetGames()
        {
            return _context.Games.ToList();
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

        public void Save()
        {
            _context.SaveChanges();
        }
        public void SaveGame(Game gameToSave)
        {
            _context.Games?.Add(gameToSave);
            _context.SaveChanges();
        }
        public void SaveShip(Ship shipToSave)
        {
            _context.Ships?.Add(shipToSave);
            _context.SaveChanges();
        }
        public Ship CreateCustomShip(string type)
        {
            if (Game != null)
            {
                Ship shipToCreate = new Ship();
                shipToCreate.GameId = Game.Id;
                shipToCreate.Type = type;
                return shipToCreate;
            }
            return new Ship();
        }
    }
}
