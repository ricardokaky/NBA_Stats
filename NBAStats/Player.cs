using System.Collections.Generic;

namespace NBAStats
{
    public class Player
    {
        public string Name { get; set; }
        public List<Game> RecentGames { get; set; }
        public Averages Averages { get; set; }
    }
}