using System.Collections.Generic;

namespace Domain
{
    public class Game
    {
        public int id { get; set; }
        public int width { get; set; }
        public int heigth { get; set; }
        public bool isPlayerMove { get; set; }
    }
}