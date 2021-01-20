using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Cell
    {
        public int Id { get; set; }
        public string? CellId { get; set; }
        public int X  { get; set; }
        public int Y  { get; set; }
        public string? ShipName { get; set; }
        public int GameId  { get; set; }
        public bool IsHit { get; set; }
        public bool IsPlayer { get; set; }
    }
}
