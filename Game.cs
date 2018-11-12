using System;
using System.Collections.Generic;

namespace Sistemata2
{
    class Game : IEquatable<Game>, IComparable<Game>
    {
        public int Week { get; private set; }
        public Team OpponentTeam { get; private set; }
        public bool Played { get; internal set; }
        public readonly Dictionary<Metric, int> Record = new Dictionary<Metric, int>();
        public double ExpectedPoints { get; internal set; }
        public double High { get; internal set; }
        public double Low { get; internal set; }

        public Game(int week, Team opponentTeam, Boolean played)
        {
            this.Played = played;
            Week = week;
            OpponentTeam = opponentTeam;
        }

        public Game()
        {
        }

        public void AddMetric(Metric metric, int count) => this.Record.Add(metric, count);

        public override bool Equals(object obj) => Equals(obj as Game);

        public bool Equals(Game other) => other != null && Week == other.Week;

        public override int GetHashCode() => Week.GetHashCode();

        int IComparable<Game>.CompareTo(Game other)
        {
            return this.Week.CompareTo(other.Week);
        }
    }
}
