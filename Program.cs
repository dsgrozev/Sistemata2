using System.IO;

namespace Sistemata2
{
    class Program
    {
        public static int lastWeek = 17;
        static void Main(string[] args)
        {
            LeagueData leagueData = MetricValue.Init(File.ReadAllText(@"C:\ff\System.json"));
            //LeagueData leagueData = MetricValue.Init(File.ReadAllText(@"C:\ff\Doom.json"));
            WebDriver dr = new WebDriver(leagueData.leagueId);
            dr.Execute();
            Team.CalculateMedian();
            Player.CalculatePoints();
            ExcelExport.Export(leagueData.leagueId);
        }
    }
}
