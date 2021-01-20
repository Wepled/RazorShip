using System.Collections.Generic;

namespace Domain
{
    public class Ship
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public int Length { get; set; }
        public int GameId { get; set; }
        public int ShipCounter { get; set; }
        public bool IsPlayer { get; set; }
    }
}