using System.IO;

namespace Sistemata2
{
    class Program
    {
        public static int lastWeek = 17;
        static void Main()
        {
            WebDriver dr = new WebDriver();
            string[] leagues = new string[] { @"C:\ff\Doom.json" };
            for (int i = 0; i < leagues.Length; i++)
            {
                Team.Init();
                LeagueData leagueData = MetricValue.Init(File.ReadAllText(leagues[i]));
                dr.Execute(leagueData.leagueId);
                Team.CalculateMedian();
                Player.CalculatePoints();
                ExcelExport.Export(leagueData.leagueId);
                Team.Delete();
                Player.Reset();
            }
            dr.Quit();
        }
    }
}
