using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain.ViewModels;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebBattleship.Pages
{
    public class Settings : PageModel
    {
        public List<string> ShipsTypes = new List<string>() { "Patrol", "Cruiser", "Submarine", "Battleship", "Carrier" };
        private List<ShipVM> shipsForSquare = new List<ShipVM>();

        public Game? Game { get; set; }
        private readonly BattleShipDbContext _context;
        
        public bool isError = false;

        public Settings(BattleShipDbContext context)
        {
            _context = context;
        }
        public void OnGet(int? gameId, string? error) 
        {
            if (gameId != null) 
            {
                Game = _context.Games.Single(x => x.Id == gameId);
            }

            isError = error == "on";
            
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            if (Game != null) 
            {
                _context.Games?.Remove(Game);
            }
            
            Game newGame = new Game();

            newGame.Width = int.Parse(Request.Form["gameWidth"]);
            newGame.Heigth  = int.Parse(Request.Form["gameHeigth"]);
            newGame.CarrierCount = int.Parse(Request.Form["CarrierNumber"]);
            newGame.SubmarineCount = int.Parse(Request.Form["SubmarineNumber"]);
            newGame.CruiserCount = int.Parse(Request.Form["CruiserNumber"]);
            newGame.BattleshipCount = int.Parse(Request.Form["BattleshipNumber"]);
            newGame.PatrolCount = int.Parse(Request.Form["PatrolNumber"]);
            newGame.CanGoToAnother = Request.Form["can"] == "on";

            _context.Games?.Add(newGame);
            _context.SaveChanges();

            foreach (string shipType in ShipsTypes)
            {
                Ship newShip = new Ship();
                newShip.Type = shipType;
                newShip.Length = int.Parse(Request.Form[shipType + "Size"]);
                newShip.GameId = newGame.Id;
                newShip.IsPlayer = true;
                shipsForSquare.Add(new ShipVM(newShip, int.Parse(Request.Form[shipType + "Number"])));
                _context.Ships?.Add(newShip);
            }
            foreach (string shipType in ShipsTypes)
            {
                Ship newShip = new Ship();
                newShip.Type = shipType;
                newShip.Length = int.Parse(Request.Form[shipType + "Size"]);
                newShip.GameId = newGame.Id;
                newShip.IsPlayer = false;
                _context.Ships?.Add(newShip);
            }

            await _context.SaveChangesAsync();

            if (!CheckSquare( newGame.Width * newGame.Heigth, shipsForSquare, newGame.CanGoToAnother)) 
            {
                return Redirect("./Settings?gameId=" + newGame.Id + "&error=on");
            }

            return Redirect("./OriginalSettingsGame?gameId=" + newGame.Id  + "&settings=custom");
        }

        public bool CheckSquare(int square, List<ShipVM> ships, bool canTouch) 
        {
            int shipSquare = 0;
            foreach (ShipVM shipvm in ships) 
            {
                if (!canTouch)
                {
                    shipSquare += (9 + 3 * (shipvm.Ship.Length - 1)) * shipvm.MaxCount;
                }
                else 
                {
                    shipSquare += (shipvm.Ship.Length) * shipvm.MaxCount;
                }
            }
            return square > shipSquare + 20;
        }
    }
}