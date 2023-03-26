using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Support.UI;
using NBAStats.Classes;
using OfficeOpenXml;

namespace NBAStats
{
    public partial class _Default : Page
    {
        private readonly HtmlWeb Client = new HtmlWeb();
        private static HtmlDocument JogadorHtml = new HtmlDocument();
        private static List<HtmlNode> JogadorStats;
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
        private static List<Partida> Partidas;
        private static List<Tuple<string, int, string>> lstIpPorta;
        private int indexIpPortaAtual = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Browser == null)
                {
                    var options = new ChromeOptions()
                    {
                        BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                    };

                    options.AddArguments(new List<string>() { "headless", "disable-gpu", "--no-sandbox", $"user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36" });

                    Browser = new ChromeDriver(options);
                }

                ProcurarLinhasOdds();

                GerarProxies();

                ProcurarHistoricoJogadores();

                Analisar();

                Browser.Quit();
            }
        }

        public async Task<IActionResult> ProcurarLinhasOdds()
        {
            try
            {
                AcessarUrl("https://br.betano.com/sport/basquete/eua/nba/17106/");

                var partidasDisponiveis = Browser.FindElements(By.XPath("//tr[contains(@class, 'events-list__grid__event')]"));

                Partidas = new List<Partida>();

                for (int i = 0; i < partidasDisponiveis.Count(); i++)
                {
                    var hyperlink = partidasDisponiveis[i].FindElement(By.XPath(".//a[@class='GTM-event-link events-list__grid__info__main']"));

                    if (hyperlink.GetAttribute("innerText").Contains("AO VIVO"))
                    {
                        continue;
                    }

                    var href = hyperlink.GetAttribute("href");
                    var times = hyperlink.FindElements(By.XPath(".//span[@class='events-list__grid__info__main__participants__participant-name']"));
                    var timeCasa = times[0].GetAttribute("innerText");
                    var timeFora = times[1].GetAttribute("innerText");

                    var dataHora = partidasDisponiveis[i].FindElement(By.XPath(".//div[@class='events-list__grid__info__datetime']")).GetAttribute("innerText");

                    if (dataHora.Substring(0, 2) != DateTime.Now.Day.ToString() && Convert.ToInt32(dataHora.Substring(dataHora.IndexOf("\n") + 1, 2)) > 2)
                    {
                        break;
                    }

                    Partidas.Add(new Partida(dataHora.Replace("\r\n", " "), timeCasa + " x " + timeFora, href));
                }

                for (int i = 0; i < Partidas.Count(); i++)
                {
                    AcessarUrl(Partidas[i].Url + "?bt=1");

                    var aba = Browser.FindElement(By.XPath("//li[@class='events-tabs-container__tab__item GTM-1-container']")).GetAttribute("innerText");

                    if (aba == "Especiais de jogadores")
                    {
                        ProcurarOddsJogadores(Partidas[i]);

                        AcessarUrl(Partidas[i].Url + "?bt=2");

                        ProcurarOddsAlternativas(Partidas[i]);
                    }
                    else if (aba == "Linhas alternativas de jogador")
                    {
                        ProcurarOddsAlternativas(Partidas[i]);
                    }
                    else
                    {
                        continue;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AcessarUrl(string url)
        {
            Browser.Navigate().GoToUrl(url);

            var wait = new WebDriverWait(Browser, TimeSpan.FromSeconds(10));
            wait.Until(x => x.FindElement(By.TagName("body")) != null);
        }

        private void ProcurarOddsJogadores(Partida partida)
        {
            Browser.FindElement(By.XPath("//button[@class='tabs-navigation__actions__button icon--clickable']")).Click();

            var eleLinhas = Browser.FindElements(By.XPath(".//div[@class='table-layout-container']"));

            for (int i = 0; i < eleLinhas.Count(); i++)
            {
                var nomeLinha = eleLinhas[i].FindElement(By.XPath(".//div[@class='table-market-header']")).GetAttribute("innerText");

                if (nomeLinha == "Double Double" || nomeLinha == "Triple Double" || nomeLinha.StartsWith("1"))
                {
                    continue;
                }

                var rows = eleLinhas[i].FindElements(By.XPath(".//div[@class='row']"));

                for (int i2 = 0; i2 < rows.Count(); i2++)
                {
                    var nomeJogador = rows[i2].FindElement(By.XPath(".//div[@class='row-title']")).GetAttribute("innerText");
                    var valorLinha = Convert.ToDouble(rows[i2].FindElement(By.XPath(".//div[@class='handicap__single-item']")).GetAttribute("innerText").Replace(".", ","));
                    var oddOver = Convert.ToDouble(rows[i2].FindElement(By.XPath(".//div[@style='--selection-column-start:1;']")).GetAttribute("innerText").Replace(".", ","));
                    var oddUnder = Convert.ToDouble(rows[i2].FindElement(By.XPath(".//div[@style='--selection-column-start:2;']")).GetAttribute("innerText").Replace(".", ","));

                    if (!partida.Jogadores.Any(x => x.Nome == nomeJogador))
                    {
                        partida.Jogadores.Add(new Jogador(nomeJogador));
                    }

                    partida.Jogadores.Find(x => x.Nome == nomeJogador).Linhas.Add(new Linha(nomeLinha, valorLinha, oddOver, oddUnder));
                }
            }
        }

        private void ProcurarOddsAlternativas(Partida partida)
        {
            var eleLinhas = Browser.FindElements(By.XPath("//div[@class='markets__market']"));

            for (int i = 0; i < eleLinhas.Count(); i++)
            {
                var aux = eleLinhas[i].FindElement(By.XPath(".//div[@class='markets__market__header__title']")).GetAttribute("innerText");
                var nomeJogador = aux.Substring(0, aux.IndexOf(" Total"));
                var nomeLinha = aux.Substring(aux.IndexOf(" Total") + 1);

                if (!partida.Jogadores.Any(x => x.Nome == nomeJogador))
                {
                    partida.Jogadores.Add(new Jogador(nomeJogador));
                }

                var botoes = eleLinhas[i].FindElements(By.XPath(".//button[contains(@class, 'selections__selection')]"));

                var linhas = new List<Linha>();

                for (int i2 = 0; i2 < botoes.Count(); i2++)
                {
                    var valor = botoes[i2].FindElement(By.XPath(".//span[@class='selections__selection__title']")).GetAttribute("innerText");
                    valor = valor.Substring(0, valor.Length - 1);

                    var odd = Convert.ToDouble(botoes[i2].FindElement(By.XPath(".//span[@class='selections__selection__odd']")).GetAttribute("innerText").Replace(".", ",").Trim());

                    linhas.Add(new Linha(nomeLinha, Convert.ToDouble(valor), odd, 0));
                }

                partida.Jogadores.Find(x => x.Nome == nomeJogador).LinhasAlternativas.Add(new LinhaAlternativa(nomeLinha, linhas));
            }
        }

        public async Task<IActionResult> ProcurarHistoricoJogadores()
        {
            try
            {
                for (int i = 0; i < Partidas.Count(); i++)
                {
                    for (int i2 = 0; i2 < Partidas[i].Jogadores.Count(); i2++)
                    {
                        var jogador = Partidas[i].Jogadores[i2];

                        if (jogador.Nome == "Aarron Nesmith")
                        {
                            jogador.Nome = "Aaron Nesmith";
                        }

                        if (jogador.Historico != null && jogador.Historico.Partidas != null && jogador.Historico.Partidas.Count() > 0)
                        {
                            continue;
                        }

                        HtmlDocument search = new HtmlDocument();
                        string href = "";

                        while (true)
                        {
                            try
                            {
                                search = Client.Load("https://www.basketball-reference.com/search/search.fcgi?search=" + jogador.Nome.Replace("Sr.", "").Replace(".", "").Replace("'", "").Replace("III", "").Trim().Replace(" ", "+").Replace("-", "+"), lstIpPorta[indexIpPortaAtual].Item1, lstIpPorta[indexIpPortaAtual].Item2, "", "");
                                href = search.DocumentNode.SelectSingleNode("//link[@rel = 'canonical']").Attributes.First(x => x.Name == "href").Value;
                                break;
                            }
                            catch (Exception)
                            {
                                if (indexIpPortaAtual == lstIpPorta.Count() - 1)
                                {
                                    GerarProxies();
                                    indexIpPortaAtual = 0;
                                }
                                else
                                {
                                    indexIpPortaAtual++;
                                }

                                continue;
                            }
                        }

                        string url;

                        if (href.EndsWith("html"))
                        {
                            url = href.Replace(".html", "/gamelog/2023");
                        }
                        else
                        {
                            url = search.DocumentNode.SelectSingleNode("//div[@class = 'search-item-url']").InnerText.Replace(".html", "/gamelog/2023").Insert(0, "https://www.basketball-reference.com");
                        }

                        if (!AcessarPaginaJogador(url))
                        {
                            return null;
                        }

                        if (!ProcurarStatsJogador())
                        {
                            return null;
                        }

                        jogador.Historico = new HistoricoJogador();

                        JogadorStats.Reverse();

                        ProcessarStats(jogador);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GerarProxies()
        {
            var sslProxies = Client.Load("https://www.sslproxies.org/");

            var rows = sslProxies.DocumentNode.SelectNodes("//table[@class='table table-striped table-bordered']//tbody//tr");

            lstIpPorta = new List<Tuple<string, int, string>>();

            for (int i = 0; i < rows.Count(); i++)
            {
                var pais = rows[i].SelectSingleNode(".//td[3]").InnerText;

                if (pais == "BR" || pais == "US")
                {
                    var ip = rows[i].SelectSingleNode(".//td[1]").InnerText;
                    var porta = rows[i].SelectSingleNode(".//td[2]").InnerText;
                    var ipPorta = new Tuple<string, int, string>(ip, Convert.ToInt32(porta), pais);
                    lstIpPorta.Add(ipPorta);
                }
            }

            lstIpPorta = lstIpPorta.OrderBy(x => x.Item3).ToList();
        }

        protected void butProcurar_Click(object sender, EventArgs e)
        {
            lblQuantJogosObrigatorio.Visible = false;
            lblMinMinutosValido.Visible = false;

            var quantJogos = txtQuantJogos.Text;

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
        }

        private bool AcessarPaginaJogador(string url)
        {
            while (true)
            {
                try
                {
                    JogadorHtml = Client.Load(url, lstIpPorta[indexIpPortaAtual].Item1, lstIpPorta[indexIpPortaAtual].Item2, "", "");
                    break;
                }
                catch (Exception)
                {
                    if (indexIpPortaAtual == lstIpPorta.Count() - 1)
                    {
                        GerarProxies();
                        indexIpPortaAtual = 0;
                    }
                    else
                    {
                        indexIpPortaAtual++;
                    }

                    continue;
                }
            }

            if (JogadorHtml.DocumentNode.ChildNodes.Count == 0)
            {
                AcessarUrl(url);
                JogadorHtml.LoadHtml(Browser.PageSource);
            }

            return JogadorHtml.DocumentNode.ChildNodes.Count > 0;
        }

        private bool ProcurarStatsJogador()
        {
            JogadorStats = new List<HtmlNode>(JogadorHtml.DocumentNode.SelectNodes("//tbody//tr[@id]").ToList());

            return JogadorStats.Count > 0;
        }

        private void ProcessarStats(Jogador jogador)
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

            jogador.Historico.Partidas.Clear();
            jogador.Historico.PartidasContraAdv.Clear();

            var quant = JogadorStats.Count();

            for (int i = 0; i < quant; i++)
            {
                var node = JogadorStats[i];

                var childNodes = node.ChildNodes.Where(x => x.Name == "td" && x.Attributes.Where(y => y.Name == "data-stat" && lstAttributes.Contains(y.Value)).Count() > 0).ToList();

                var nodeMinutos = childNodes.Find(x => x.Attributes.Where(y => y.Value == "mp").Count() > 0);

                if (!string.IsNullOrEmpty(txtMinMinutos.Text) && Convert.ToInt32(nodeMinutos.InnerText.Substring(0, nodeMinutos.InnerText.IndexOf(":"))) < Convert.ToInt32(txtMinMinutos.Text))
                {
                    quant++;
                    continue;
                }

                jogador.Historico.Partidas.Add(ProcessarPartida(childNodes));
            }

            if (!string.IsNullOrEmpty(ddlAdversario.SelectedValue))
            {
                ProcessarStatsAdversario(jogador);
            }
            else
            {
                lblPartidasContraAdv.Visible = false;
                lblMediasContraAdv.Visible = false;
                ucPartidasContraAdv.Visible = false;
                ucMediasContraAdv.Visible = false;
            }

            ProcessarDuploETriploDuplo(jogador);
        }

        private void ProcessarDuploETriploDuplo(Jogador jogador)
        {
            lblDoubleDouble.Visible = true;
            lblTripleDouble.Visible = true;

            double porcentDuploDuplo = (jogador.Historico.Partidas.Where(x => x.DuploDuplo).Count() * 100) / jogador.Historico.Partidas.Count();
            double porcentTriploDuplo = (jogador.Historico.Partidas.Where(x => x.TriploDuplo).Count() * 100) / jogador.Historico.Partidas.Count();

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

        private void ProcessarStatsAdversario(Jogador jogador)
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
                    jogador.Historico.PartidasContraAdv.Add(ProcessarPartida(childNodes));
                }

                ucPartidasContraAdv.Prepara(jogador.Historico.PartidasContraAdv);

                if (quantJogosAdv > 1)
                {
                    lblMediasContraAdv.Visible = true;
                    lblMediasContraAdv.Text = "Médias Contra " + ddlAdversario.SelectedItem.Text;

                    ucMediasContraAdv.Prepara(jogador.Historico.PartidasContraAdv, jogador.Historico.MediasContraAdv);
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

        private void Analisar()
        {
            for (int i = 0; i < Partidas.Count(); i++)
            {
                for (int i2 = 0; i2 < Partidas[i].Jogadores.Count(); i2++)
                {
                    var partida = Partidas[i];
                    var jogador = partida.Jogadores[i2].Nome;

                    var lstPontos = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.Pontos).ToList();
                    var lstRebotes = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.Rebotes).ToList();
                    var lstAssistencias = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.Assistencias).ToList();
                    var lstCestas3Feitas = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.Cestas3Feitas).ToList();
                    var lstRoubos = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.Roubos).ToList();
                    var lstBloqueios = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.Bloqueios).ToList();
                    var lstPontosRebotesAssistencias = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.PontosAssistenciasRebotes).ToList();

                    foreach (Linha linha in Partidas[i].Jogadores[i2].Linhas)
                    {
                        switch (linha.Nome)
                        {
                            case "Pontos Mais/Menos":
                                VerificaOcorrenciasLinha(lstPontos, linha);
                                break;
                            case "Rebotes Mais/Menos":
                                VerificaOcorrenciasLinha(lstRebotes, linha);
                                break;
                            case "Assistências Mais/Menos":
                                VerificaOcorrenciasLinha(lstAssistencias, linha);
                                break;
                            case "Total Arremessos de três pontos Marcados +/-":
                                VerificaOcorrenciasLinha(lstCestas3Feitas, linha);
                                break;
                            case "Roubos Mais/Menos":
                                VerificaOcorrenciasLinha(lstRoubos, linha);
                                break;
                            case "Bloqueios Mais/Menos":
                                VerificaOcorrenciasLinha(lstBloqueios, linha);
                                break;
                            case "Turnover Mais/Menos":
                                var lstTurnovers = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.InversoesPosse).ToList();
                                VerificaOcorrenciasLinha(lstTurnovers, linha);
                                break;
                            case "Pontos + Rebotes + Assistências Mais/Menos":
                                VerificaOcorrenciasLinha(lstPontosRebotesAssistencias, linha);
                                break;
                            case "Pontos + Rebotes Mais/Menos":
                                var lstPontosRebotes = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.PontosRebotes).ToList();
                                VerificaOcorrenciasLinha(lstPontosRebotes, linha);
                                break;
                            case "Pontos + Assistências Mais/Menos":
                                var lstPontosAssistencias = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.PontosAssistencias).ToList();
                                VerificaOcorrenciasLinha(lstPontosAssistencias, linha);
                                break;
                            case "Rebotes + Assistências Mais/Menos":
                                var lstRebotesAssistencias = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.AssistenciasRebotes).ToList();
                                VerificaOcorrenciasLinha(lstRebotesAssistencias, linha);
                                break;
                            case "Pontos + Bloqueios Mais/Menos":
                                var lstPontosBloqueios = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.PontosBloqueios).ToList();
                                VerificaOcorrenciasLinha(lstPontosBloqueios, linha);
                                break;
                            case "Roubadas + Bloqueios Mais/Menos":
                                var lstRoubosBloqueios = Partidas[i].Jogadores[i2].Historico.Partidas.Select(x => x.RoubosBloqueios).ToList();
                                VerificaOcorrenciasLinha(lstRoubosBloqueios, linha);
                                break;
                            default:
                                break;
                        }
                    }

                    foreach (LinhaAlternativa linhaAlternativa in Partidas[i].Jogadores[i2].LinhasAlternativas)
                    {
                        switch (linhaAlternativa.Nome)
                        {
                            case "Total de Pontos":
                                VerificaOcorrenciasLinhaAlternativa(lstPontos, linhaAlternativa.Linhas);
                                break;
                            case "Total de Rebotes":
                                VerificaOcorrenciasLinhaAlternativa(lstRebotes, linhaAlternativa.Linhas);
                                break;
                            case "Total de Assistências":
                                VerificaOcorrenciasLinhaAlternativa(lstAssistencias, linhaAlternativa.Linhas);
                                break;
                            case "Total Arremessos de três pontos Marcados":
                                VerificaOcorrenciasLinhaAlternativa(lstCestas3Feitas, linhaAlternativa.Linhas);
                                break;
                            case "Total de Roubos":
                                VerificaOcorrenciasLinhaAlternativa(lstRoubos, linhaAlternativa.Linhas);
                                break;
                            case "Total de tocos":
                                VerificaOcorrenciasLinhaAlternativa(lstBloqueios, linhaAlternativa.Linhas);
                                break;
                            case "Total de Pontos, Rebotes e Assistências":
                                VerificaOcorrenciasLinhaAlternativa(lstPontosRebotesAssistencias, linhaAlternativa.Linhas);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(@"C:\Users\ricardo.queiroz\Downloads\Linhas Assertivas.xls"))
            {
                for (int i = 0; i < Partidas.Count(); i++)
                {
                    var sheet = package.Workbook.Worksheets.Add(Partidas[i].Times);

                    int index = 1;

                    for (int i2 = 0; i2 < Partidas[i].Jogadores.Count(); i2++)
                    {
                        for (int i3 = 0; i3 < Partidas[i].Jogadores[i2].Linhas.Count(); i3++)
                        {
                            sheet.Cells[$"A{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                            sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Nome;
                            sheet.Cells[$"C{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Valor;

                            if (Partidas[i].Jogadores[i2].Linhas[i3].SequenciaUnder > 0)
                            {
                                sheet.Cells[$"D{index}"].Value = "SIM";
                                sheet.Cells[$"E{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddUnder.ToString().Replace(".", ",");
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaUnder;
                            }
                            else
                            {
                                sheet.Cells[$"E{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddOver.ToString().Replace(".", ",");
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaOver;
                            }

                            index++;
                        }

                        for (int i3 = 0; i3 < Partidas[i].Jogadores[i2].LinhasAlternativas.Where(x => x.Linhas.Where(y => y.SequenciaOver > 0).Count() > 0).Count(); i3++)
                        {
                            var linhasSequencia = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Linhas.Where(x => x.SequenciaOver > 0).ToList();

                            for (int i4 = 0; i4 < linhasSequencia.Count(); i4++)
                            {
                                var linha = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Linhas.Find(x => x.Nome == linhasSequencia[i4].Nome && x.Valor == linhasSequencia[i4].Valor);

                                sheet.Cells[$"A{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                                sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Nome;
                                sheet.Cells[$"C{index}"].Value = linha.Valor;
                                sheet.Cells[$"E{index}"].Value = linha.OddOver.ToString().Replace(".", ",");
                                sheet.Cells[$"F{index}"].Value = linha.SequenciaOver;

                                index++;
                            }
                        }
                    }
                }

                package.Save();
            }
        }

        private void VerificaOcorrenciasLinha(List<int> lista, Linha linha)
        {
            if (lista[0] > linha.Valor)
            {
                if (lista.Any(x => x < linha.Valor))
                {
                    linha.SequenciaOver = lista.IndexOf(lista.First(x => x < linha.Valor));
                }
                else
                {
                    linha.SequenciaOver = lista.Count();
                }
            }
            else
            {
                if (lista.Any(x => x > linha.Valor))
                {
                    linha.SequenciaUnder = lista.IndexOf(lista.First(x => x > linha.Valor));
                }
                else
                {
                    linha.SequenciaUnder = lista.Count();
                }
            }
        }

        private void VerificaOcorrenciasLinhaAlternativa(List<int> lista, List<Linha> linhas)
        {
            for (int i = 0; i < linhas.Count(); i++)
            {
                if (lista.Any(x => x < linhas[i].Valor))
                {
                    linhas[i].SequenciaOver = lista.IndexOf(lista.First(x => x < linhas[i].Valor));
                }
                else
                {
                    linhas[i].SequenciaOver = lista.Count();
                }
            }
        }
    }
}