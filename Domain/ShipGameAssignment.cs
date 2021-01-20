using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ShipGameAssignment
    {

        public int id { get; set; }
        public int Game_Id { get; set; }
        public List<Cell>? Cells { get; set; }
        public string? ShipName { get; set; }
        public int Length { get; set; }
        public string? Type { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsRotated { get; set; } = false;
    }
}
