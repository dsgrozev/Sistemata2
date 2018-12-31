using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sistemata2
{
    public class Player
    {
        public static List<Player> Players = new List<Player>();

        public string Name { get; private set; }
        public Team Team { get; private set; }
        public Position Pos { get; private set; }
        public string Owner { get; private set; }

        internal List<Game> Games = new List<Game>();
        internal readonly Dictionary<Metric, double> ratings = new Dictionary<Metric, double>();
        private readonly Dictionary<Metric, double> lows = new Dictionary<Metric, double>();
        private readonly Dictionary<Metric, double> highs = new Dictionary<Metric, double>();

        public Player(string name, Position pos, string owner, Team team)
        {
            Name = name;
            Team = team;
            Pos = pos;
            Owner = owner;
        }

        internal void AddGame(Game game)
        {
            if(!this.Games.Contains(game))
                this.Games.Add(game);
        }

        internal static int MinWeek()
        {
            int minWeek = 20;
            foreach(Player def in Players.Where(x => x.Pos == Position.DEF))
            {
                var futureGames = def.Games.Where(x => x.Played == false);
                if (futureGames.Count() > 0)
                    minWeek = Math.Min(futureGames.Min().Week, minWeek);
            }
            return minWeek;
        }

        internal void AddMetricGame(Game game)
        {
            Game g = this.Games.Find(x => x.Week == game.Week);
            foreach (KeyValuePair<Metric, int> metric in game.Record)
            {
                g.AddMetric(metric.Key, metric.Value);
            }
        }

        internal void AddGames(IEnumerable<Game> games)
        {
            foreach (Game g in games)
            {
                this.Games.Add(new Game(g.Week, g.OpponentTeam, g.Played, g.HomeGame));
            }
        }
        internal static void CalculatePoints()
        {
            Console.Write("Calculate Players:");
            foreach (Player player in Player.Players)
            {
                player.Calculate();
                Console.Write(".");
            }
            Console.WriteLine();
        }

        internal Game GetGame(int week)
        {
            Game ret = this.Games.FindLast(x => x.Week == week);
            if (ret == null)
            {
                return new Game();
            }
            return ret;
        }

        private void Calculate()
        {
            if (this.Games.Count(x => x.Played == true) > 1)
            {
                this.CalculateRatings();
                this.CalculateFuturePoints();
            }
        }

        private void CalculateFuturePoints()
        {
            foreach (Game game in this.Games.Where(x => x.Played == false))
            {
                if (game.ExpectedPoints != 0)
                {
                    throw new Exception();
                }
                foreach(Metric metric in this.ratings.Keys)
                {
                    if (metric != Metric.PtsVs)
                    {
                        double points = MetricValue.GetPoints(metric);
                        game.ExpectedPoints += this.ratings[metric] * game.OpponentTeam.means[metric] * points;
                        game.Low += this.lows[metric] * game.OpponentTeam.means[metric] * points;
                        game.High += this.highs[metric] * game.OpponentTeam.means[metric] * points;
                    }
                    else
                    {
                        game.ExpectedPoints += GetDefPoints(this.ratings[metric] * game.OpponentTeam.means[metric]);
                        game.Low += GetDefPoints(this.lows[metric] * game.OpponentTeam.means[metric]);
                        game.High += GetDefPoints(this.highs[metric] * game.OpponentTeam.means[metric]);
                    }

                }
            }
        }

        private int GetDefPoints(double points)
        {
            if (points < .5)
                return 10;
            if (points < 6.5)
                return 7;
            if (points < 13.5)
                return 4;
            if (points < 20.5)
                return 1;
            if (points < 27.5)
                return 0;
            if (points < 34.5)
                return -1;
            return -4;
        }

        private void CalculateRatings()
        {
            Dictionary<Metric, List<double>> temp = new Dictionary<Metric, List<double>>();
            foreach(Game game in this.Games.Where(x => x.Played == true))
            {
                foreach(KeyValuePair<Metric, int> pair in game.Record)
                {
                    if (!temp.ContainsKey(pair.Key))
                    {
                        temp.Add(pair.Key, new List<double>());
                    }
                    var oppMean = game.OpponentTeam.means[pair.Key];
                    if (oppMean != 0)
                        temp[pair.Key].Add(1.0 * pair.Value / oppMean);
                    else
                        temp[pair.Key].Add(0);
                }
            }
            foreach(Metric metric in temp.Keys)
            {
                double std = CalculateStdDev(temp[metric]);
                double mean = CalcAdvMean(temp[metric]);
                //double mean = temp[metric].Average();
                this.lows[metric] = mean - std;
                this.highs[metric] = mean + std;
                this.ratings[metric] = mean;
            }
        }

        internal double CalcAdvMean(List<double> list)
        {
            int arraySize = 0;
            if (list.Count < 3)
                return list.Average();
            else if (list.Count < 5)
                arraySize = list.Count + 2;
            else
                arraySize = list.Count + 8;

            double[] arr = new double[arraySize];
            list.CopyTo(arr);

            if (list.Count < 5) {
                arr[list.Count] = list[list.Count - 2];
                arr[list.Count + 1] = list[list.Count - 1];
            }
            else
            {
                arr[list.Count] = list[list.Count - 4];
                arr[list.Count + 1] = list[list.Count - 3];
                for (int j = 0; j < 3; j++)
                {
                    arr[list.Count + 2 + j] = list[list.Count - 2];
                }
                for (int j = 0; j < 3; j++)
                {
                    arr[list.Count + 5 + j] = list[list.Count - 1];
                }
            }
            return arr.Average();
        }

        //private double Mean(List<double> list)
        //{
        //    if (list.Count == 0)
        //        return 0;
        //    list.Sort();
        //    int size = list.Count;
        //    int mid = size / 2;
        //    return (size % 2 != 0) ? list[mid] : (list[mid] + list[mid - 1]) / 2;
        //}

        private double CalculateStdDev(List<double> values)
        {
            double ret = 0;
            if (values.Count() > 1)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
    }
}
