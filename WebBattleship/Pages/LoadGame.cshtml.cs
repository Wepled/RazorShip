using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebBattleship.Pages
{
    public class LoadGame : PageModel
    {
        private readonly BattleShipDbContext _context;

        public LoadGame(BattleShipDbContext context)
        {
            _context = context;
        }

        public List<Game>? Games { get; set; }

        public async Task OnGetAsync(int? GameId)
        {
            if (GameId != null)
            {
                _context.Games?.Remove(_context.Games.Single(x => x.Id == GameId.Value));
                _context.Cells?.RemoveRange(_context.Cells.Where(x => x.GameId == GameId.Value));
                _context.Ships?.RemoveRange(_context.Ships.Where(x => x.GameId == GameId.Value));
                _context.SaveChanges();
            }

            Games = await _context.Games.ToListAsync();
        }
        
        
    }
}