namespace Domain
{
    public class Move
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public bool isPlayerMove { get; set; }
    }
}