using System.Collections.Generic;

namespace Sistemata2
{
    class TeamWeek
    {
        public int WeekNumber { get; private set; }
        public Dictionary<Metric, int> results = new Dictionary<Metric, int>();
        public TeamWeek(int weekNumber) => WeekNumber = weekNumber;

        public void AddMetric(Metric metric, int count)
        {
            if (this.results.ContainsKey(metric))
            {
                this.results[metric] += count;
            }
            else
            {
                this.results.Add(metric, count);
            }
        }
    }
}
