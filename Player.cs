using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sistemata2
{
    public class Player
    {
        public string Name { get; private set; }
        public Team Team { get; private set; }
        public Position Pos { get; private set; }
        public string Owner { get; private set; }

        internal List<Game> Games = new List<Game>();

        public Player(string name, Team team, Position pos, string owner)
        {
            Name = name;
            Team = team;
            Pos = pos;
            Owner = owner;
        }

        internal void AddGame(Game game)
        {
            Debug.Assert(!this.Games.Contains(game));
            this.Games.Add(game);
        }
    }
}
