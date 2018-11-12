namespace Sistemata2
{
    class Program
    {
        static void Main(string[] args)
        {
            WebDriver dr = new WebDriver("849675", "2018");
            dr.Execute();
            Team.CalculateMedian();
            Player.CalculatePoints();
            ExcelExport.Export();
        }
    }
}
