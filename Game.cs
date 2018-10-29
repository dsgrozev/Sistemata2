using System;
using System.Collections.Generic;

namespace Sistemata2
{
    class Game : IEquatable<Game>
    {
        public int Week { get; private set; }
        public Team OpponentTeam { get; private set; }

        public readonly Dictionary<Metric, int> Record = new Dictionary<Metric, int>();
        public readonly Dictionary<Metric, double> Ratings = new Dictionary<Metric, double>();

        public Game(int week, Team opponentTeam)
        {
            Week = week;
            OpponentTeam = opponentTeam;
        }

        public void AddMetric(Metric metric, int count) => this.Record.Add(metric, count);

        public override bool Equals(object obj) => Equals(obj as Game);

        public bool Equals(Game other) => other != null && Week == other.Week;

        public override int GetHashCode() => Week.GetHashCode();
    }
}
