using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace Sistemata2
{
    class WebDriver
    {
        private FirefoxDriver driver = null;
        private string leagueNumber;

        public WebDriver(string leagueNumber)
        {
            this.driver = this.StartDriver(); ;
            this.leagueNumber = leagueNumber;
        }

        public void Execute()
        {
            CalcPlayerRatings("DEF", false);
            CalcPlayerRatings("O", true);
            CalcPlayerRatings("K", true);
            driver.Quit();
        }

        private void CalcPlayerRatings(string position, bool onlyPoints)
        {
            int lastWeek = 16;
            if (position != "DEF")
            {
                lastWeek = Player.MinWeek();
            }
            for (int i = 1; i <= lastWeek; i++)
            {
                Console.Write(position + " : " + i.ToString() + " : ");
                int rowCount = 0;
                int count = 0;
                bool reachedEndOfWeek = false;
                do
                {
                    IWebElement table = null;
                    driver.Navigate().GoToUrl(@"https://football.fantasysports.yahoo.com/f1/" + this.leagueNumber + 
                        "/players?status=ALL&pos=" + position + "&cut_type=9&stat1=S_W_" + i + "&sort=0&sdir=1&count=" + count);
                    while (true)
                    {
                        try
                        {
                            table = new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(
                                ExpectedConditions.ElementIsVisible(By.Id("players-table-wrapper")));
                            break;
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    var rows = table.FindElements(By.TagName("tr"));
                    rowCount = rows.Count;

                    for (int j = 2; j < rowCount; j++)
                    {
                        reachedEndOfWeek = HandlePlayerRow(rows[j], i, position);
                    }

                    count += 25;

                    if (reachedEndOfWeek && onlyPoints)
                    {
                        break;
                    }

                    Console.Write(".");

                } while (rowCount == 27);
                Console.WriteLine();
            }
        }

        private bool HandlePlayerRow(IWebElement row, int week, string position)
        {
            string rowText = row.Text;
            var fields = rowText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var player = fields[1];

            int i = 3;
            while (fields[i].Length != 1)
                i++;

            var oppString = fields[i - 1];
            string oppTeam = "";

            bool homeGame = true;
            if (oppString == "Bye")
            {
                oppTeam = "Bye";
            }
            else if (oppString.Contains("vs"))
            {
                oppTeam = oppString.Split(new string[] { "vs" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            }
            else
            {
                oppTeam = oppString.Split('@')[1].Trim();
                homeGame = false;
            }

            if (!Char.IsLetter(oppTeam[oppTeam.Length - 1]))
                oppTeam = oppTeam.Substring(0, oppTeam.Length - 1);

            if (fields[i + 1].StartsWith("Video"))
                i++;

            var owner = fields[i + 1];
            var gamesPlayed = fields[i + 2];

            var nameParts = player.Split(' ');
            string shortTeamName = nameParts[nameParts.Length - 3];
            string pos = nameParts[nameParts.Length - 1];
            Team team = Team.Teams.Find(x => x.ShortName == shortTeamName);

            Player p = Player.Players.Find(x => x.Name == player);
            if (p == null)
            {
                p = new Player(player, (Position)Enum.Parse(typeof(Position), pos), owner, team);
                if (position != "DEF")
                {
                    var def = Player.Players.Find(x => x.Team == team && x.Pos == Position.DEF);
                    p.AddGames(def.Games.Where(x => x.Played == false));
                }
                Player.Players.Add(p);
            }

            var opp = Team.Teams.Find(x => x.ShortName == oppTeam);

            if (oppTeam != "Bye" && position == "DEF" && gamesPlayed != "1")
            {
                p.AddGame(new Game(week, opp, false, homeGame));
            }

            if (gamesPlayed == "1" && oppTeam != "Bye")
            {
                Game game = new Game(week, opp, true, homeGame);
                UpdateGame(game, position, fields);
                p.AddGame(game);
                opp.AddGame(game);
            }

            return gamesPlayed != "1";
        }

        private void UpdateGame(Game game, string position, string[] fields)
        {
            if (position == "DEF")
            {
                UpdateDefGame(game, fields);
            }
            else if (position == "K")
            {
                UpdateKickGame(game, fields);
            }
            else
            {
                UpdateOffGame(game, fields);
            }
        }

        private void UpdateOffGame(Game game, string[] fields)
        {
            int i = fields.Length - 13;
            bool fum = false;
            if (MetricValue.METRICS.FindIndex(x => x.TextName == "Fum") >= 0)
            {
                fum = true;
                i--;
            }
            game.AddMetric(Metric.PassYds, ExtractValue(fields, i++));
            game.AddMetric(Metric.PassTD, ExtractValue(fields, i++));
            game.AddMetric(Metric.PassInt, ExtractValue(fields, i++));
            game.AddMetric(Metric.Att, ExtractValue(fields, i++));
            game.AddMetric(Metric.RushYds, ExtractValue(fields, i++));
            game.AddMetric(Metric.RushTD, ExtractValue(fields, i++));
            game.AddMetric(Metric.Tgt, ExtractValue(fields, i++));
            game.AddMetric(Metric.Rec, ExtractValue(fields, i++));
            game.AddMetric(Metric.RecYds, ExtractValue(fields, i++));
            game.AddMetric(Metric.RecTD, ExtractValue(fields, i++));
            game.AddMetric(Metric.RetTD, ExtractValue(fields, i++));
            game.AddMetric(Metric.TwoPt, ExtractValue(fields, i++));
            if (fum)
            {
                game.AddMetric(Metric.Fum, ExtractValue(fields, i++));
            }
            game.AddMetric(Metric.FumLost, ExtractValue(fields, i++));
        }

        private int ExtractValue(string[] fields, int i)
        {
            int temp = 0;
            if (int.TryParse(fields[i], out temp))
            {
                return temp;
            }
            return 0;
        }

        private void UpdateKickGame(Game game, string[] fields)
        {
            int i = fields.Length - 6;
            game.AddMetric(Metric.FG19, int.Parse(fields[i++]));
            game.AddMetric(Metric.FG29, int.Parse(fields[i++]));
            game.AddMetric(Metric.FG39, int.Parse(fields[i++]));
            game.AddMetric(Metric.FG49, int.Parse(fields[i++]));
            game.AddMetric(Metric.FG50, int.Parse(fields[i++]));
            game.AddMetric(Metric.PAT, int.Parse(fields[i++]));
        }

        private void UpdateDefGame(Game game, string[] fields)
        {
            int i = fields.Length - 8;
            game.AddMetric(Metric.PtsVs, int.Parse(fields[i++]));
            game.AddMetric(Metric.Sack, int.Parse(fields[i++]));
            game.AddMetric(Metric.Safe, int.Parse(fields[i++]));
            game.AddMetric(Metric.DefInt, int.Parse(fields[i++]));
            game.AddMetric(Metric.FumRec, int.Parse(fields[i++]));
            game.AddMetric(Metric.DefTD, int.Parse(fields[i++]));
            game.AddMetric(Metric.BlkKick, int.Parse(fields[i++]));
            game.AddMetric(Metric.DefRetTD, int.Parse(fields[i++]));
        }

        private FirefoxDriver StartDriver()
        {
            FirefoxOptions ffo = new FirefoxOptions();
            ffo.AddArguments(new[] {"--private"});
            ffo.SetPreference("pageLoadStrategy", "eager");
            while (driver == null)
            {
                try
                {
                    driver = new FirefoxDriver(ffo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            driver.Url = "https://login.yahoo.com";

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var elem = wait.Until(ExpectedConditions.ElementToBeClickable((By.CssSelector("input.phone-no#login-username")))); ;
            elem.SendKeys("dsgrozev@hotmail.com");
            //driver.FindElementById("login-signin").Click();
            return driver;
        }

        //private ChromeDriver StartDriver()
        //{
        //    //ChromeOptions opt = new ChromeOptions
        //    //{
        //    //    Proxy = null
        //    //};
        //    //opt.AddArgument(@"load-extension=3.4.2_0\Adblock-Plus_v1.12.4.crx");
            
        //    ChromeDriver driver = new ChromeDriver(@"C:\Users\Dimitar\Downloads\selenium-dotnet-3.5.1\")
        //    {
        //        Url = "https://login.yahoo.com"
        //    };
        //    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        //    var elem = wait.Until(ExpectedConditions.ElementToBeClickable((By.CssSelector("input.phone-no#login-username")))); ;
        //    elem.SendKeys("dsgrozev@hotmail.com");
        //    driver.FindElementById("login-signin").Click();
        //    //Console.ReadLine();
        //    return driver;
        //}
    }
}
