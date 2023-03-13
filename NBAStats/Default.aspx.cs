using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using NBAStats.Classes;
using OpenQA.Selenium.Interactions;

namespace NBAStats
{
    public partial class _Default : Page
    {
        private readonly HtmlWeb Client = new HtmlWeb();
        private static HtmlDocument Html;
        private static HtmlDocument JogadorHtml;
        private static List<HtmlNode> JogadorStats;
        private static HistoricoJogador Jogador;
        private int QuantJogos;
        private static string NomeJogador;
        private WebDriver Browser;
        private readonly List<string> lstAtributos = new List<string>()
        {
            "date_game",
            "team_id",
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

        protected void Page_Load(object sender, EventArgs e)
        {
            ProcurarOdds();
        }

        public async Task<IActionResult> ProcurarOdds()
        {
            try
            {
                string fullUrl = "https://br.betano.com/sport/basquete/eua/nba/17106/";
                List<string> programmerLinks = new List<string>();

                var options = new ChromeOptions()
                {
                    BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                };

                options.AddArguments(new List<string>() { "headless", "disable-gpu", "--no-sandbox", $"user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36" });

                Browser = new ChromeDriver(options);
                AcessarUrl(fullUrl);

                var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(10));
                wait.Until(x => x.FindElement(By.TagName("body")) != null);

                var partidasDisponiveis = Browser.FindElements(By.XPath("//tr[contains(@class, 'events-list__grid__event')]"));

                var lstPartidas = new List<Partida>();

                // loopa as partidas listadas
                for (int i = 0; i < partidasDisponiveis.Count(); i++)
                {
                    var hyperlink = partidasDisponiveis[i].FindElement(By.XPath(".//a[@class='GTM-event-link events-list__grid__info__main']"));

                    var teste = hyperlink.Text;

                    // pula as partidas ao vivo
                    if (hyperlink.Text.Contains("AO VIVO"))
                    {
                        continue;
                    }

                    var href = hyperlink.GetAttribute("href");
                    var times = hyperlink.FindElements(By.XPath(".//span[@class='events-list__grid__info__main__participants__participant-name']"));
                    var timeCasa = times[0].Text;
                    var timeFora = times[1].Text;

                    var dataHora = partidasDisponiveis[i].FindElement(By.XPath(".//div[@class='events-list__grid__info__datetime']")).Text;

                    var partida = new Partida(dataHora, timeCasa + " x " + timeFora);

                    ProcurarOddsJogadores(ref partida, href + "?bt=1");

                    ProcurarOddsAlternativas(ref partida, href + "?bt=2");

                    lstPartidas.Add(partida);
                }

                Browser.Quit();

                return null;
            }
            catch (Exception ex)
            {
                Browser.Quit();
                throw ex;
            }
        }

        private void AcessarUrl(string url)
        {
            Browser.Navigate().GoToUrl(url);

            var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(10));
            wait.Until(x => x.FindElement(By.TagName("body")) != null);
        }

        private void ProcurarOddsJogadores(ref Partida partida, string url)
        {
            AcessarUrl(url);

            // clica no botão para expandir todas as linhas
            Browser.FindElement(By.XPath("//button[@class='tabs-navigation__actions__button icon--clickable']")).Click();

            var botoes = Browser.FindElements(By.XPath("//button[@class='load-more']"));

            for (int i = 0; i < botoes.Count(); i++)
            {
                botoes[i].SendKeys(Keys.Return);
            }

            var source = Browser.PageSource;

            var eleLinhas = Browser.FindElements(By.XPath(".//div[@class='table-layout-container']"));

            // loopa as linhas disponíveis
            for (int i = 0; i < eleLinhas.Count(); i++)
            {
                var nomeLinha = eleLinhas[i].FindElement(By.XPath(".//div[@class='table-market-header']")).Text;

                // pula duplo duplo e triplo duplo
                if (nomeLinha == "Double Double" || nomeLinha == "Triple Double")
                {
                    continue;
                }

                var rows = eleLinhas[i].FindElements(By.XPath(".//div[@class='row']"));

                // loopa os jogadores disponíveis
                for (int i2 = 0; i2 < rows.Count(); i2++)
                {
                    var nomeJogador = rows[i2].FindElement(By.XPath(".//div[@class='row-title']")).Text;
                    var valorLinha = rows[i2].FindElement(By.XPath(".//div[@class='handicap__single-item']")).Text;

                    if (!partida.Jogadores.Any(x => x.Nome == nomeJogador))
                    {
                        partida.Jogadores.Add(new Jogador(nomeJogador));
                    }

                    partida.Jogadores.First(x => x.Nome == nomeJogador).Linhas.Add(new Linha(nomeLinha, valorLinha));
                }
            }
        }

        private void ProcurarOddsAlternativas(ref Partida partida, string url)
        {
            AcessarUrl(url);
        }

        protected void butProcurar_Click(object sender, EventArgs e)
        {
            try
            {
                lblNomeJogadorObrigatorio.Visible = false;
                lblQuantJogosObrigatorio.Visible = false;
                lblMinMinutosValido.Visible = false;

                var nomeJogador = txtNomeJogador.Text;
                var quantJogos = txtQuantJogos.Text;

                if (string.IsNullOrEmpty(nomeJogador))
                {
                    lblNomeJogadorObrigatorio.Visible = true;
                    return;
                }
                else
                {
                    lblNomeJogadorObrigatorio.Visible = false;
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

                if (NomeJogador != txtNomeJogador.Text)
                {
                    if (!ProcurarJogador(nomeJogador))
                    {
                        return;
                    }

                    if (!AcessarPaginaJogador(nomeJogador))
                    {
                        return;
                    }

                    if (!ProcurarStatsJogador())
                    {
                        return;
                    }

                    JogadorStats.Reverse();
                }

                ProcessarStats();

                NomeJogador = txtNomeJogador.Text;
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

            Jogador = new HistoricoJogador(nomeJogador);

            return JogadorHtml.DocumentNode.ChildNodes.Count > 0;
            //Falha ao acessar estatísticas do {nomeJogador}!
        }

        private bool ProcurarStatsJogador()
        {
            JogadorStats = new List<HtmlNode>(JogadorHtml.DocumentNode.SelectNodes("//tbody//tr[@id]").ToList());

            return JogadorStats.Count > 0;
            //Falha ao acessar o front das estatísticas!
        }

        private void ProcessarStats()
        {
            var lstAttributes = new List<string>()
            {
                "date_game",
                "team_id",
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

            Jogador.Partidas.Clear();
            Jogador.PartidasContraAdv.Clear();

            for (int i = 0; i < QuantJogos; i++)
            {
                var node = JogadorStats[i];

                var childNodes = node.ChildNodes.Where(x => x.Name == "td" && x.Attributes.Where(y => y.Name == "data-stat" && lstAttributes.Contains(y.Value)).Count() > 0).ToList();

                var nodeMinutos = childNodes.Find(x => x.Attributes.Where(y => y.Value == "mp").Count() > 0);

                if (!string.IsNullOrEmpty(txtMinMinutos.Text) && Convert.ToInt32(nodeMinutos.InnerText.Substring(0, nodeMinutos.InnerText.IndexOf(":"))) < Convert.ToInt32(txtMinMinutos.Text))
                {
                    QuantJogos++;
                    continue;
                }

                Jogador.Partidas.Add(ProcessarPartida(childNodes));
            }

            lblTrocaTime.Visible = Jogador.Partidas.Select(x => x.Time).Distinct().Count() > 1;

            lblPartidasRecentes.Visible = true;
            lblMediasPartidasRecentes.Visible = true;

            ucPartidas.Prepara(Jogador.Partidas);

            ucMedias.Prepara(Jogador.Partidas, Jogador.Medias);

            if (!string.IsNullOrEmpty(ddlAdversario.SelectedValue))
            {
                ProcessarStatsAdversario();
            }
            else
            {
                lblPartidasContraAdv.Visible = false;
                lblMediasContraAdv.Visible = false;
                ucPartidasContraAdv.Visible = false;
                ucMediasContraAdv.Visible = false;
            }

            ProcessarDuploETriploDuplo();
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

        private PartidaJogador ProcessarPartida(List<HtmlNode> nodes)
        {
            var dicStats = new Dictionary<string, string>()
                    {
                        { "Data", null },
                        { "Time", null },
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

            for (int j = 0; j < nodes.Count(); j++)
            {
                string text = nodes[j].InnerText;

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

            return PartidaJogador.DictionaryDePara(dicStats);
        }

        private void ProcessarStatsAdversario()
        {
            var nodes = JogadorStats.Where(z => z.ChildNodes.Where(x => x.Name == "td" && x.Attributes.Where(y => y.Value == "opp_id").Count() > 0 && x.InnerText == ddlAdversario.SelectedValue).Count() > 0).ToList();

            int quantJogosAdv = nodes.Count();

            if (quantJogosAdv > 0)
            {
                lblPartidasContraAdv.Visible = true;
                lblPartidasContraAdv.Text = "Partidas Contra " + ddlAdversario.SelectedItem.Text;

                for (int i = 0; i < quantJogosAdv; i++)
                {
                    var childNodes = nodes[i].ChildNodes.Where(x => x.Name == "td" && x.Attributes.Where(y => y.Name == "data-stat" && lstAtributos.Contains(y.Value)).Count() > 0).ToList();
                    Jogador.PartidasContraAdv.Add(ProcessarPartida(childNodes));
                }

                ucPartidasContraAdv.Prepara(Jogador.PartidasContraAdv);

                if (quantJogosAdv > 1)
                {
                    lblMediasContraAdv.Visible = true;
                    lblMediasContraAdv.Text = "Médias Contra " + ddlAdversario.SelectedItem.Text;

                    ucMediasContraAdv.Prepara(Jogador.PartidasContraAdv, Jogador.MediasContraAdv);
                }
                else
                {
                    ucMediasContraAdv.Visible = false;
                    lblMediasContraAdv.Visible = false;
                }
            }
            else
            {
                ucPartidasContraAdv.Visible = false;
                ucMediasContraAdv.Visible = false;
                lblPartidasContraAdv.Visible = false;
                lblMediasContraAdv.Visible = false;
            }
        }
    }
}