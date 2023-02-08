using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

namespace NBAStats
{
    public partial class _Default : Page
    {
        private HtmlWeb Client = new HtmlWeb();
        private HtmlDocument Html;
        private HtmlDocument PlayerHtml;
        private List<HtmlNode> PlayerStats;
        private Player Player;
        private int QuantJogos;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void butSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //trocar de textbox para um select com a lista dos nomes dos jogadores
                lblPlayerNameRequired.Visible = false;
                lblQuantJogosObrigatorio.Visible = false;

                var playerName = txtPlayerName.Text;
                var quantJogos = txtQuantJogos.Text;

                if (string.IsNullOrEmpty(playerName))
                {
                    lblPlayerNameRequired.Visible = true;
                    return;
                }
                else
                {
                    lblPlayerNameRequired.Visible = false;
                }

                if (string.IsNullOrEmpty(quantJogos) || quantJogos.Any(x => char.IsLetter(x)))
                {
                    lblQuantJogosObrigatorio.Visible = true;
                    return;
                }
                else
                {
                    lblQuantJogosObrigatorio.Visible = false;
                }

                QuantJogos = Convert.ToInt32(quantJogos);

                if (!AccessSite())
                {
                    return;
                }

                if (!SearchPlayer(playerName))
                {
                    return;
                }

                if (!AccessPlayerPage(playerName))
                {
                    return;
                }

                if (!SearchPlayerStats())
                {
                    return;
                }

                ProcessStats();
            }
            catch (WebException ex)
            {
                //exibir alert com exception
                //falha ao acessar site
                throw;
            }
            catch (Exception ex)
            {
                //exibir alert com exception
                throw;
            }
        }

        private bool AccessSite()
        {
            if (Html == null)
            {
                Html = Client.Load("https://www.basketball-reference.com/leagues/NBA_2023_advanced.html");
                //Html = Client.Load("https://www.espn.com/nba/stats/player");

                return Html.DocumentNode.ChildNodes.Count > 0;
                //"Falha ao acessar site!"
            }

            return true;
        }

        private bool SearchPlayer(string nomeJogador)
        {
            return Html.DocumentNode.SelectSingleNode($"//a[text() = \"{nomeJogador}\"]") != null;
            //"Falha ao procurar pelo jogador!"
        }

        private bool AccessPlayerPage(string nomeJogador)
        {
            var link = Html.DocumentNode.SelectSingleNode($"//a[text() = \"{nomeJogador}\"]").Attributes["href"].Value;
            link = link.Replace(".html", "/gamelog/2023").Insert(0, "https://www.basketball-reference.com");

            PlayerHtml = Client.Load(link);

            Player = new Player
            {
                Name = nomeJogador
            };

            return PlayerHtml.DocumentNode.ChildNodes.Count > 0;
            //Falha ao acessar estatísticas do {nomeJogador}!
        }

        private bool SearchPlayerStats()
        {
            //PlayerStats = PlayerHtml.DocumentNode.SelectSingleNode("//section[@class = 'Card gamelogWidget gamelogWidget--basketball']//tbody[@class='Table__TBODY']");
            PlayerStats = new List<HtmlNode>(PlayerHtml.DocumentNode.SelectNodes("//tbody//tr[@id]").ToList());
            return PlayerStats.Count > 0;
            //Falha ao acessar o front das estatísticas!
        }

        private void ProcessStats()
        {
            Player.RecentGames = new List<Game>();

            PlayerStats.Reverse();

            var lstAttributes = new List<string>()
                {
                    "date_game",
                    "opp_id",
                    "game_result",
                    "mp",
                    "fg",
                    "fga",
                    "fg3",
                    "fg3a",
                    "fg3_pct",
                    "ft",
                    "fta",
                    "ft_pct",
                    "trb",
                    "ast",
                    "stl",
                    "blk",
                    "tov",
                    "pts"
                };

            for (int i = 0; i < QuantJogos; i++)
            {
                var node = PlayerStats[i];

                var childNodes = node.ChildNodes.Where(x => x.Name == "td" && x.Attributes.Where(y => y.Name == "data-stat" && lstAttributes.Contains(y.Value)).Count() > 0).ToList();

                var dicStats = new Dictionary<string, string>()
                    {
                        { "Date", null },
                        { "Opponent", null },
                        { "Result", null },
                        { "Minutes", null },
                        { "FieldGoalsMade", null },
                        { "FieldGoalsAttempted", null },
                        { "TreePointFieldGoalsMade", null },
                        { "TreePointFieldGoalsAttempted", null },
                        { "TreePointFieldGoalPercentage", null },
                        { "FreeThrowsMade", null },
                        { "FreeThrowsAttempted", null },
                        { "FreeThrowPercentage", null },
                        { "Rebounds", null },
                        { "Assists", null },
                        { "Steals", null },
                        { "Blocks", null },
                        { "Turnovers", null },
                        { "Points", null }
                    };

                for (int j = 0; j < childNodes.Count; j++)
                {
                    string text = childNodes[j].InnerText;

                    if (string.IsNullOrEmpty(text))
                    {
                        text = "0";
                    }
                    else if (text.StartsWith("."))
                    {
                        text = "0" + text;
                    }

                    dicStats[dicStats.ElementAt(j).Key] = text;
                }

                var game = new Game();
                game.FromDictionary(dicStats);

                Player.RecentGames.Add(game);
            }

            dgGames.DataSource = Player.RecentGames;
            dgGames.DataBind();

            ProcessAverages();

            ProcessDoubleAndTripleDouble();
        }

        private void ProcessAverages()
        {
            Player.Averages = new Averages
            {
                AvgTreePointFieldGoalsMade = Math.Round(Player.RecentGames.Select(x => x.TreePointFieldGoalsMade).Average(), 1),
                AvgRebounds = Math.Round(Player.RecentGames.Select(x => x.Rebounds).Average(), 1),
                AvgAssists = Math.Round(Player.RecentGames.Select(x => x.Assists).Average(), 1),
                AvgBlocks = Math.Round(Player.RecentGames.Select(x => x.Blocks).Average(), 1),
                AvgSteals = Math.Round(Player.RecentGames.Select(x => x.Steals).Average(), 1),
                AvgTurnovers = Math.Round(Player.RecentGames.Select(x => x.Turnovers).Average(), 1),
                AvgPoints = Math.Round(Player.RecentGames.Select(x => x.Points).Average(), 1),
                AvgPointsAssistsRebounds = Math.Round(Player.RecentGames.Select(x => x.PointsAssistsRebounds).Average(), 1),
                AvgPointsAssists = Math.Round(Player.RecentGames.Select(x => x.PointsAssists).Average(), 1),
                AvgPointsRebounds = Math.Round(Player.RecentGames.Select(x => x.PointsRebounds).Average(), 1),
                AvgAssistsRebounds = Math.Round(Player.RecentGames.Select(x => x.AssistsRebounds).Average(), 1),
                AvgStealsBlocks = Math.Round(Player.RecentGames.Select(x => x.StealsBlocks).Average(), 1)
            };

            List<Averages> lstAverages = new List<Averages>()
            {
                Player.Averages
            };

            dgAverages.DataSource = lstAverages;
            dgAverages.DataBind();
        }

        private void ProcessDoubleAndTripleDouble()
        {
            if (Player.RecentGames.Where(x => x.DoubleAndTripleDouble >= 3).Count() >= Player.RecentGames.Count() / 2)
            {
                lblDoubleDouble.Visible = true;
                lblTripleDouble.Visible = true;
            }
            else if (Player.RecentGames.Where(x => x.DoubleAndTripleDouble >= 2).Count() >= Player.RecentGames.Count() / 2)
            {
                lblDoubleDouble.Visible = true;
            }
        }

        protected void dgAverages_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                foreach (PropertyInfo property in Player.Averages.GetType().GetProperties())
                {
                    Label lblAverage = (Label)e.Item.FindControl(property.Name);

                    double average = Convert.ToDouble(property.GetValue(Player.Averages));

                    double variancia = 0;

                    foreach (var value in Player.RecentGames.Select(x => x.GetType().GetProperty(property.Name.Replace("Avg", "")).GetValue(x)))
                    {
                        variancia += Math.Pow((Convert.ToDouble(value) - average), 2);
                    }

                    double desvioPadrao = Math.Sqrt(variancia / (Player.RecentGames.Count() - 1));

                    double coefVariacao = (desvioPadrao / average) * 100;

                    if (average < 1 || coefVariacao > 30)
                    {
                        lblAverage.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (coefVariacao > 15 && coefVariacao <= 30)
                    {
                        lblAverage.ForeColor = System.Drawing.Color.Blue;
                    }
                    else
                    {
                        lblAverage.ForeColor = System.Drawing.Color.LightGreen;
                    }
                }
            }
        }
    }
}