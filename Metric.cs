using System.Collections.Generic;

namespace Sistemata2
{
    public enum Position
    {
        QB,
        RB,
        WR,
        TE,
        K,
        DEF
    }

    public enum Metric
    {
        PassYds,
        PassTD,
        PassInt,
        Att,
        RushYds,
        RushTD,
        Tgt,
        Rec,
        RecYds,
        RecTD,
        RetTD,
        TwoPt,
        FumLost,
        FG19,
        FG29,
        FG39,
        FG49,
        FG50,
        PAT,
        PtsVs,
        Sack,
        DefInt,
        FumRec,
        DefTD,
        Safe,
        BlkKick
    }

    class MetricValue
    {
        public Metric Name { get; private set; }
        public double Points { get; private set; }
        public MetricValue(Metric name, double points)
        {
            this.Name = name;
            this.Points = points;
        }

        private static readonly List<MetricValue> METRICS = new List<MetricValue>()
        {
            new MetricValue(Metric.PassYds, .04),
            new MetricValue(Metric.PassTD, 4),
            new MetricValue(Metric.PassInt, -1),
            new MetricValue(Metric.Att, 0),
            new MetricValue(Metric.RushYds, .1),
            new MetricValue(Metric.RushTD, 6),
            new MetricValue(Metric.Tgt, 0),
            new MetricValue(Metric.Rec, .5),
            new MetricValue(Metric.RecYds, .1),
            new MetricValue(Metric.RecTD, 6),
            new MetricValue(Metric.RetTD, 6),
            new MetricValue(Metric.TwoPt, 2),
            new MetricValue(Metric.FumLost, -2),
            new MetricValue(Metric.FG19, 3),
            new MetricValue(Metric.FG29, 3),
            new MetricValue(Metric.FG39, 3),
            new MetricValue(Metric.FG49, 4),
            new MetricValue(Metric.FG50, 5),
            new MetricValue(Metric.PAT, 1),
            new MetricValue(Metric.Sack, 1),
            new MetricValue(Metric.DefInt, 2),
            new MetricValue(Metric.FumRec, 2),
            new MetricValue(Metric.DefTD, 6),
            new MetricValue(Metric.Safe, 2),
            new MetricValue(Metric.BlkKick, 2)
        };

        internal static double GetPoints(Metric metric)
        {
            return METRICS.Find(x => x.Name == metric).Points;
        }
    }
}

