using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{

    public class BattleShipDbContext : DbContext

    {

        public DbSet<Game>? Games { get; set; }
        public DbSet<Ship>? Ships { get; set; }
        public DbSet<Cell>? Cells { get; set; }
        public DbSet<ShipGameAssignment>? ShipGameAssignments { get; set; }

        public BattleShipDbContext(DbContextOptions<BattleShipDbContext> options) : base(options)
        {
        }
    }
}