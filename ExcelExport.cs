using System;
using System.Linq;

namespace Sistemata2
{
    static class ExcelExport
    {
        public static void Export(string leagueId)
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
            workSheet.Cells[1, col++] = "Team";
            foreach (string m in Enum.GetNames(typeof(Metric)))
            {
                workSheet.Cells[1, col++] = m;
            }

            col = 1;
            row = 2;
            foreach (Player p in Player.Players)
            {
                Console.Write(".");
                workSheet.Cells[row, col++] = p.Name;
                workSheet.Cells[row, col++] = p.Pos.ToString();
                workSheet.Cells[row, col++] = p.Team.Name;
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

                int minWeek = Player.MinWeek();

                for (int i = minWeek; i < Program.lastWeek + 1; i++)
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

                    for (int i = minWeek; i < Program.lastWeek + 1; i++)
                    {
                        workSheet.Cells[row, col++] = p.GetGame(i).ExpectedPoints;
                    }
                    row++;
                }

                //excel.Worksheets.Add();
                //workSheet = excel.ActiveSheet;
                //workSheet.Name = "Offence Data";

                //col = 1;
                //workSheet.Cells[1, col++] = "Player";
                //workSheet.Cells[1, col++] = "Position";
                //workSheet.Cells[1, col++] = "Team";
                //workSheet.Cells[1, col++] = "vs Team";
                //workSheet.Cells[1, col++] = "Week Number";
                //workSheet.Cells[1, col++] = "Home Game?";
                //for (Metric i = Metric.PassYds; i <= Metric.FumLost; i++)
                //{
                //    workSheet.Cells[1, col++] = i.ToString();
                //}

                //row = 2; // start row (in row 1 are header cells)
                //foreach (Player p in Player.Players.Where(x => x.Pos != Position.DEF && x.Pos != Position.K))
                //{
                //    foreach (Game g in p.Games)
                //    {
                //        col = 1;
                //        workSheet.Cells[row, col++] = p.Name;
                //        workSheet.Cells[row, col++] = p.Pos.ToString();
                //        workSheet.Cells[row, col++] = p.Team.Name;
                //        workSheet.Cells[row, col++] = g.OpponentTeam.Name;
                //        workSheet.Cells[row, col++] = g.Week;
                //        workSheet.Cells[row, col++] = g.HomeGame.ToString();
                //        for (Metric i = Metric.PassYds; i <= Metric.FumLost; i++)
                //        {
                //            workSheet.Cells[row, col++] = g.Record[i].ToString();
                //        }
                //        row++;
                //    }
                //}
                //excel.Worksheets.Add();
                //workSheet = excel.ActiveSheet;
                //workSheet.Name = "Kicker Data";

                //col = 1;
                //workSheet.Cells[1, col++] = "Player";
                //workSheet.Cells[1, col++] = "Team";
                //workSheet.Cells[1, col++] = "vs Team";
                //workSheet.Cells[1, col++] = "Week Number";
                //workSheet.Cells[1, col++] = "Home Game?";
                //for (Metric i = Metric.FG19; i <= Metric.PAT; i++)
                //{
                //    workSheet.Cells[1, col++] = i.ToString();
                //}

                //row = 2; // start row (in row 1 are header cells)
                //foreach (Player p in Player.Players.Where(x => x.Pos == Position.K))
                //{
                //    foreach (Game g in p.Games)
                //    {
                //        col = 1;
                //        workSheet.Cells[row, col++] = p.Name;
                //        workSheet.Cells[row, col++] = p.Team.Name;
                //        workSheet.Cells[row, col++] = g.OpponentTeam.Name;
                //        workSheet.Cells[row, col++] = g.Week;
                //        workSheet.Cells[row, col++] = g.HomeGame.ToString();
                //        for (Metric i = Metric.FG19; i <= Metric.PAT; i++)
                //        {
                //            workSheet.Cells[row, col++] = g.Record[i].ToString();
                //        }
                //        row++;
                //    }
                //}

                //excel.Worksheets.Add();
                //workSheet = excel.ActiveSheet;
                //workSheet.Name = "Defence Data";

                //col = 1;
                //workSheet.Cells[1, col++] = "Player";
                //workSheet.Cells[1, col++] = "Team";
                //workSheet.Cells[1, col++] = "vs Team";
                //workSheet.Cells[1, col++] = "Week Number";
                //workSheet.Cells[1, col++] = "Home Game?";
                //for (Metric i = Metric.PtsVs; i <= Metric.DefRetTD; i++)
                //{
                //    workSheet.Cells[1, col++] = i.ToString();
                //}

                //row = 2; // start row (in row 1 are header cells)
                //foreach (Player p in Player.Players.Where(x => x.Pos == Position.DEF))
                //{
                //    foreach (Game g in p.Games)
                //    {
                //        col = 1;
                //        workSheet.Cells[row, col++] = p.Name;
                //        workSheet.Cells[row, col++] = p.Team.Name;
                //        workSheet.Cells[row, col++] = g.OpponentTeam.Name;
                //        workSheet.Cells[row, col++] = g.Week;
                //        workSheet.Cells[row, col++] = g.HomeGame.ToString();
                //        for (Metric i = Metric.PtsVs; i <= Metric.DefRetTD; i++)
                //        {
                //            workSheet.Cells[row, col++] = g.Record[i].ToString();
                //        }
                //        row++;
                //    }
                //}

                // Save this data as a file
                workSheet.SaveAs(@"c:\ff\" + leagueId + "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + ".xlsx");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
