using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Data;

namespace NBAStats
{
    public partial class _Default : Page
    {
        private HtmlWeb Client = new HtmlWeb();
        private HtmlDocument Html;
        private HtmlDocument JogadorHtml;
        private List<HtmlNode> JogadorStats;
        private Jogador Jogador;
        private int QuantJogos;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void butSearch_Click(object sender, EventArgs e)
        {
            try
            {
                lblPlayerNameRequired.Visible = false;
                lblQuantJogosObrigatorio.Visible = false;
                lblMinMinutosValido.Visible = false;

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

                if (txtMinMinutos.Text.Any(x => char.IsLetter(x)))
                {
                    lblMinMinutosValido.Visible = true;
                    return;
                }
                else
                {
                    lblMinMinutosValido.Visible = false;
                }

                QuantJogos = Convert.ToInt32(quantJogos);

                if (!AcessarSite())
                {
                    return;
                }

                if (!ProcurarJogador(playerName))
                {
                    return;
                }

                if (!AcessarPaginaJogador(playerName))
                {
                    return;
                }

                if (!ProcurarStatsJogador())
                {
                    return;
                }

                ProcessarStats();
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

        private bool AcessarSite()
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

        private bool ProcurarJogador(string nomeJogador)
        {
            return Html.DocumentNode.SelectSingleNode($"//a[text() = \"{nomeJogador}\"]") != null;
            //"Falha ao procurar pelo jogador!"
        }

        private bool AcessarPaginaJogador(string nomeJogador)
        {
            var link = Html.DocumentNode.SelectSingleNode($"//a[text() = \"{nomeJogador}\"]").Attributes["href"].Value;
            link = link.Replace(".html", "/gamelog/2023").Insert(0, "https://www.basketball-reference.com");

            JogadorHtml = Client.Load(link);

            Jogador = new Jogador(nomeJogador);

            return JogadorHtml.DocumentNode.ChildNodes.Count > 0;
            //Falha ao acessar estatísticas do {nomeJogador}!
        }

        private bool ProcurarStatsJogador()
        {
            //PlayerStats = PlayerHtml.DocumentNode.SelectSingleNode("//section[@class = 'Card gamelogWidget gamelogWidget--basketball']//tbody[@class='Table__TBODY']");
            JogadorStats = new List<HtmlNode>(JogadorHtml.DocumentNode.SelectNodes("//tbody//tr[@id]").ToList());
            return JogadorStats.Count > 0;
            //Falha ao acessar o front das estatísticas!
        }

        private void ProcessarStats()
        {
            JogadorStats.Reverse();

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
                var node = JogadorStats[i];

                var childNodes = node.ChildNodes.Where(x => x.Name == "td" && x.Attributes.Where(y => y.Name == "data-stat" && lstAttributes.Contains(y.Value)).Count() > 0).ToList();

                if (!string.IsNullOrEmpty(txtMinMinutos.Text) && Convert.ToInt32(childNodes[3].InnerText.Substring(0, childNodes[3].InnerText.IndexOf(":"))) < Convert.ToInt32(txtMinMinutos.Text))
                {
                    QuantJogos++;
                    continue;
                }

                var dicStats = new Dictionary<string, string>()
                    {
                        { "Data", null },
                        { "Adversario", null },
                        { "Resultado", null },
                        { "Minutos", null },
                        { "FieldGoalsFeitos", null },
                        { "FieldGoalsTentados", null },
                        { "Cestas3Feitas", null },
                        { "Cestas3Tentadas", null },
                        { "Cestas3Percentual", null },
                        { "LancesLivresFeitos", null },
                        { "LancesLivresTentados", null },
                        { "LancesLivresPercentual", null },
                        { "Rebotes", null },
                        { "Assistencias", null },
                        { "Roubos", null },
                        { "Bloqueios", null },
                        { "InversoesPosse", null },
                        { "Pontos", null }
                    };

                for (int j = 0; j < childNodes.Count(); j++)
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

                var partida = new Partida();
                partida.DictionaryDePara(dicStats);

                Jogador.Partidas.Add(partida);
            }

            dgGames.DataSource = Jogador.Partidas;
            dgGames.DataBind();

            ProcessarMedias();

            ProcessarDuploETriploDuplo();
        }

        private void ProcessarMedias()
        {
            DataTable dt = new DataTable();

            foreach (var media in Jogador.Medias)
            {
                dt.Columns.Add(media.Nome);
            }

            DataRow dr = dt.NewRow();

            for (int i = 0; i < Jogador.Medias.Count(); i++)
            {
                Jogador.Medias[i].Valor = Math.Round(Jogador.Partidas.Select(y => (int)y.GetType().GetProperty(Jogador.Medias[i].Nome).GetValue(y)).Average(), 1);
                ProcessarCoeficienteVariacao(Jogador.Medias[i]);

                dr[i] = Jogador.Medias[i].Valor;
            }

            dt.Rows.Add(dr);

            dgAverages.DataSource = dt;
            dgAverages.DataBind();
        }

        private void ProcessarCoeficienteVariacao(Media media)
        {
            double variancia = 0;

            foreach (var value in Jogador.Partidas.Select(x => x.GetType().GetProperty(media.Nome).GetValue(x)))
            {
                variancia += Math.Pow((Convert.ToDouble(value) - media.Valor), 2);
            }

            double desvioPadrao = Math.Sqrt(variancia / (Jogador.Partidas.Count() - 1));

            media.CoeficienteVariacao = Math.Round((desvioPadrao / media.Valor) * 100, 2);
        }

        private void ProcessarDuploETriploDuplo()
        {
            lblDoubleDouble.Visible = true;
            lblTripleDouble.Visible = true;

            double porcentDuploDuplo = (Jogador.Partidas.Where(x => x.DuploDuplo).Count() * 100) / Jogador.Partidas.Count();
            double porcentTriploDuplo = (Jogador.Partidas.Where(x => x.TriploDuplo).Count() * 100) / Jogador.Partidas.Count();

            if (porcentDuploDuplo <= 40)
            {
                lblDoubleDouble.ForeColor = System.Drawing.Color.Red;
            }
            else if (porcentDuploDuplo > 40 && porcentDuploDuplo <= 60)
            {
                lblDoubleDouble.ForeColor = System.Drawing.Color.Blue;
            }
            else
            {
                lblDoubleDouble.ForeColor = System.Drawing.Color.LightGreen;
            }

            if (porcentTriploDuplo <= 40)
            {
                lblTripleDouble.ForeColor = System.Drawing.Color.Red;
            }
            else if (porcentTriploDuplo > 40 && porcentTriploDuplo <= 60)
            {
                lblTripleDouble.ForeColor = System.Drawing.Color.Blue;
            }
            else
            {
                lblTripleDouble.ForeColor = System.Drawing.Color.LightGreen;
            }
        }

        protected void dgAverages_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                foreach (Media media in Jogador.Medias)
                {
                    Label lblAverage = (Label)e.Item.FindControl("lblAvg" + media.Nome);

                    if (media.Valor < 1 || media.CoeficienteVariacao > 30)
                    {
                        lblAverage.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (media.CoeficienteVariacao > 15 && media.CoeficienteVariacao <= 30)
                    {
                        lblAverage.ForeColor = System.Drawing.Color.Blue;
                    }
                    else
                    {
                        lblAverage.ForeColor = System.Drawing.Color.LightGreen;
                    }

                    Label coefMedia = (Label)e.Item.FindControl("coefAvg" + media.Nome);
                    coefMedia.Text = $"({media.CoeficienteVariacao})";
                }
            }
        }
    }
}