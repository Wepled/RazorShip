using System.Collections.Generic;

namespace Domain
{
    public class Game
    {
        public int Id { get; set; }
        public string? Game_Name { get; set; }
        public int Width { get; set; }
        public int Heigth { get; set; }
        public bool IsPlayerMove { get; set; }
        public int CarrierCount { get; set; }
        public int BattleshipCount { get; set; }
        public int SubmarineCount { get; set; }
        public int CruiserCount { get; set; }
        public int PatrolCount { get; set; }
        public bool CanGoToAnother { get; set; } = false;
        public List<Cell>? Cells { get; set; }
        public List<Ship>? Ships { get; set; }
    }
}