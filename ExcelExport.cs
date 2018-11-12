using System;
using System.Linq;

namespace Sistemata2
{
    static class ExcelExport
    {
        public static void Export()
        {
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();

            excel.Workbooks.Add();
            Microsoft.Office.Interop.Excel._Worksheet workSheet = excel.ActiveSheet;
            workSheet.Name = "Teams";

            int col = 1;
            workSheet.Cells[1, col++] = "Name";

            foreach(string m in Enum.GetNames(typeof(Metric)))
            {
                workSheet.Cells[1, col++] = m;
            }

            col = 1;
            int row = 2;
            foreach(Team t in Team.Teams)
            {
                workSheet.Cells[row, col++] = t.Name;
                foreach (string m in Enum.GetNames(typeof(Metric)))
                {
                    var metr = (Metric)Enum.Parse(typeof(Metric), m);
                    if (t.means.ContainsKey(metr))
                    {
                        workSheet.Cells[row, col++] = t.means[metr];
                    }
                }
                row++;
                col = 1;
            }

            excel.Worksheets.Add();
            workSheet = excel.ActiveSheet;
            workSheet.Name = "Players_Raw";

            col = 1;
            workSheet.Cells[1, col++] = "Name";
            workSheet.Cells[1, col++] = "Position";
            foreach (string m in Enum.GetNames(typeof(Metric)))
            {
                workSheet.Cells[1, col++] = m;
            }

            col = 1;
            row = 2;
            foreach (Player p in Player.Players)
            {
                workSheet.Cells[row, col++] = p.Name;
                workSheet.Cells[row, col++] = p.Pos;
                foreach (string m in Enum.GetNames(typeof(Metric)))
                {
                    var metr = (Metric)Enum.Parse(typeof(Metric), m);
                    if (p.ratings.ContainsKey(metr))
                    {
                        workSheet.Cells[row, col++] = p.ratings[metr];
                    }
                    else
                    {
                        workSheet.Cells[row, col++] = 0;
                    }
                }
                row++;
                col = 1;
            }

            excel.Worksheets.Add();
            workSheet = excel.ActiveSheet;
            workSheet.Name = "Players";
            try
            {
                col = 1;
                workSheet.Cells[1, col++] = "Name";
                workSheet.Cells[1, col++] = "Owner";
                workSheet.Cells[1, col++] = "Position";
                workSheet.Cells[1, col++] = "Rest of season";
                workSheet.Cells[1, col++] = "LowThisWeek";
                workSheet.Cells[1, col++] = "HighThisWeek";
                workSheet.Cells[1, col++] = "LowNextWeek";
                workSheet.Cells[1, col++] = "HighNextWeek";

                int minWeek = Player.MinWeek();

                for (int i = minWeek; i < 17; i++)
                {
                    workSheet.Cells[1, col++] = "Week " + i;
                }

                row = 2; // start row (in row 1 are header cells)
                foreach (Player p in Player.Players)
                {
                    col = 1;
                    workSheet.Cells[row, col++] = p.Name;
                    workSheet.Cells[row, col++] = p.Owner;
                    workSheet.Cells[row, col++] = p.Pos.ToString();
                    workSheet.Cells[row, col++] = p.Games.Sum(x => x.ExpectedPoints);
                    workSheet.Cells[row, col++] = p.GetGame(minWeek).Low;
                    workSheet.Cells[row, col++] = p.GetGame(minWeek).High;
                    workSheet.Cells[row, col++] = p.GetGame(minWeek + 1).Low;
                    workSheet.Cells[row, col++] = p.GetGame(minWeek + 1).High;

                    for (int i = minWeek; i < 17; i++)
                    {
                        workSheet.Cells[row, col++] = p.GetGame(i).ExpectedPoints;
                    }
                    row++;
                }

                // Save this data as a file
                workSheet.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Fantasy\FantasyWeek" + DateTime.Now.ToShortDateString().Replace('/', '_') + ".xlsx");
            }
            catch (Exception)
            {
            }
            finally
            {
                // Quit Excel application
                excel.Quit();

                // Release COM objects (very important!)
                if (excel != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                if (workSheet != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

                // Empty variables
                excel = null;
                workSheet = null;

                // Force garbage collector cleaning
                GC.Collect();
            }

        }
    }
}
