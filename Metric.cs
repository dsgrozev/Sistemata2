using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        Fum,
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
        BlkKick,
        DefRetTD
    }

    public class MetricValue
    {
        public Metric Name { get; private set; }
        public double Points { get; private set; }
        public string TextName;
        public MetricValue(Metric name, double points)
        {
            this.Name = name;
            this.TextName = name.ToString();
            this.Points = points;
        }
        public static List<MetricValue> METRICS;
        public static LeagueData Init(string jsonFile)
        {
            LeagueData ld = 
                new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(jsonFile)), 
                                    typeof(LeagueData)) as LeagueData;

            METRICS = new List<MetricValue>(ld.metrics);
            return ld;
        }
        
        //{
        //    new MetricValue(Metric.PassYds, .04),
        //    new MetricValue(Metric.PassTD, 4),
        //    new MetricValue(Metric.PassInt, -1),
        //    new MetricValue(Metric.Att, 0),
        //    new MetricValue(Metric.RushYds, .1),
        //    new MetricValue(Metric.RushTD, 6),
        //    new MetricValue(Metric.Tgt, 0),
        //    new MetricValue(Metric.Rec, 1),
        //    new MetricValue(Metric.RecYds, .1),
        //    new MetricValue(Metric.RecTD, 6),
        //    new MetricValue(Metric.RetTD, 6),
        //    new MetricValue(Metric.TwoPt, 2),
        //    new MetricValue(Metric.FumLost, -2),
        //    new MetricValue(Metric.FG19, 3),
        //    new MetricValue(Metric.FG29, 3),
        //    new MetricValue(Metric.FG39, 3),
        //    new MetricValue(Metric.FG49, 4),
        //    new MetricValue(Metric.FG50, 5),
        //    new MetricValue(Metric.PAT, 1),
        //    new MetricValue(Metric.Sack, 1),
        //    new MetricValue(Metric.DefInt, 2),
        //    new MetricValue(Metric.FumRec, 2),
        //    new MetricValue(Metric.DefTD, 6),
        //    new MetricValue(Metric.Safe, 2),
        //    new MetricValue(Metric.BlkKick, 2),
        //    new MetricValue(Metric.DefRetTD, 6)
        //};

        internal static double GetPoints(Metric metric)
        {
            return METRICS.Find(x => x.Name == metric).Points;
        }
    }
}

