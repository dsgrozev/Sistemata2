using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistemata2
{
    public class Team
    {
        public readonly static List<Team> Teams = new List<Team>()
        {
            new Team("Arizona Cardinals", "Ari"),
            new Team("Atlanta Falcons", "Atl"),
            new Team("Baltimore Ravens", "Bal"),
            new Team("Buffalo Bills", "Buf"),
            new Team("Carolina Panthers", "Car"),
            new Team("Chicago Bears", "Chi"),
            new Team("Cincinnati Bengals", "Cin"),
            new Team("Cleveland Browns", "Cle"),
            new Team("Dallas Cowboys", "Dal"),
            new Team("Denver Broncos", "Den"),
            new Team("Detroit Lions", "Det"),
            new Team("Green Bay Packers", "GB"),
            new Team("Houston Texans", "Hou"),
            new Team("Indianapolis Colts", "Ind"),
            new Team("Jacksonville Jaguars", "Jax"),
            new Team("Kansas City Chiefs", "KC"),
            new Team("Los Angeles Rams", "LAR"),
            new Team("Los Angeles Chargers", "LAC"),
            new Team("Miami Dolphins", "Mia"),
            new Team("Minnesota Vikings", "Min"),
            new Team("New England Patriots", "NE"),
            new Team("New Orleans Saints", "NO"),
            new Team("New York Giants", "NYG"),
            new Team("New York Jets", "NYJ"),
            new Team("Oakland Raiders", "Oak"),
            new Team("Philadelphia Eagles", "Phi"),
            new Team("Pittsburgh Steelers", "Pit"),
            new Team("San Francisco 49ers", "SF"),
            new Team("Seattle Seahawks", "Sea"),
            new Team("Tampa Bay Buccaneers", "TB"),
            new Team("Tennessee Titans", "Ten"),
            new Team("Washington Redskins", "Was")
        };

        public Team(string fullName, string shortName)
        {
            this.Name = fullName;
            this.ShortName = shortName;
        }
    
        public string Name { get; private set; }
        public string ShortName { get; private set; }

        internal readonly List<TeamWeek> weeks = new List<TeamWeek>();
        internal readonly Dictionary<Metric, double> means = new Dictionary<Metric, double>();

        internal void AddGame(Game game)
        {
            TeamWeek week = this.weeks.Find(x => x.WeekNumber == game.Week);
            if (week == null)
            {
                week = new TeamWeek(game.Week);
                this.weeks.Add(week);
            }
            foreach(KeyValuePair<Metric, int> metric in game.Record)
            {
                week.AddMetric(metric.Key, metric.Value);
            }
        }

        internal static void CalculateMedian()
        {
            Console.Write("Calculate Teams:");
            foreach (Team t in Team.Teams)
            {
                foreach (Metric m in Enum.GetValues(typeof(Metric)))
                {
                    t.means.Add(m, t.Median(m));
                }
                Console.Write(".");
            }
            Console.WriteLine();
        }

        private double Median(Metric metric)
        {
            List<int> values = new List<int>();
            foreach (TeamWeek week in this.weeks)
            {
                if (week.results.ContainsKey(metric))
                {
                    values.Add(week.results[metric]);
                }
            }
            if (values.Count == 0)
            {
                return 0;
            }
            return CalcAdvMean(values);//values.Average();
        }

        private double CalcAdvMean(List<int> list)
        {
            int arraySize = 0;
            if (list.Count < 3)
                return list.Average();
            else if (list.Count < 5)
                arraySize = list.Count + 2;
            else
                arraySize = list.Count + 8;

            int[] arr = new int[arraySize];
            list.CopyTo(arr);

            if (list.Count < 5)
            {
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

        //internal static double Mean(List<int> values)
        //{
        //    if (values.Count == 0)
        //    {
        //        return 0;
        //    }
        //    values.Sort();
        //    if (values.Count % 2 != 0)
        //    {
        //        return values[values.Count / 2];
        //    }
        //    else
        //    {
        //        return (double)(values[values.Count / 2] + values[values.Count / 2 - 1]) / 2;
        //    }
        //}
    }
}
