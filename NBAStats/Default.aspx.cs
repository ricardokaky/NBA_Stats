using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OpenQA.Selenium;
using NBAStats.Classes;
using OfficeOpenXml;
using OpenQA.Selenium.Firefox;
using System.IO;
using System.Diagnostics;

namespace NBAStats
{
    public partial class _Default : Page
    {
        private static readonly HtmlDocument JogadorHtml = new HtmlDocument();
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
        private static string UrlEspeciaisDeJogadores;
        private static string UrlAlternativasDeJogadores;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    InstanciaDriver(false);

                    ScrapBetano();

                    ProcurarHistoricoJogadores();

                    GerarPlanilha();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    EncerraProcessos();
                }
            }
        }

        public void InstanciaDriver(bool eager)
        {
            var options = new FirefoxOptions()
            {
                BrowserExecutableLocation = @"C:\Users\ricardo.queiroz\AppData\Local\Mozilla Firefox\firefox.exe"
            };

            if (eager)
            {
                options.PageLoadStrategy = PageLoadStrategy.Eager;
            }

            options.AddArguments(new List<string>() {
                "--disable-gpu",
                "--disable-application-cache",
                "--disable-extensions",
                "disable-infobars",
                "--headless",
                "--no-sandbox",
                $"user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36" });

            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"C:\Users\ricardo.queiroz\AppData\Local\Mozilla Firefox", "geckodriver.exe");

            Browser = new FirefoxDriver(service, options);
        }

        public void ScrapBetano()
        {
            try
            {
                ProcurarPartidasDisponiveis();

                if (Partidas.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(UrlEspeciaisDeJogadores))
                    {
                        ProcurarOddsEspeciaisJogadores();
                    }

                    if (!string.IsNullOrEmpty(UrlAlternativasDeJogadores))
                    {
                        ProcurarOddsAlternativas();
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ProcurarPartidasDisponiveis()
        {
            Browser.Navigate().GoToUrl("https://br.betano.com/sport/basquete/eua/nba/17106/?bt=0");

            var partidasDisponiveis = Browser.FindElements(By.XPath("//tr[contains(@class, 'events-list__grid__event')]"));

            Partidas = new List<Partida>();

            foreach (var partida in partidasDisponiveis)
            {
                var hyperlink = partida.FindElement(By.XPath(".//a[@class='GTM-event-link events-list__grid__info__main']"));

                if (hyperlink.GetAttribute("innerText").Contains("AO VIVO"))
                {
                    continue;
                }

                var times = hyperlink.FindElements(By.XPath(".//span[@class='events-list__grid__info__main__participants__participant-name']"));
                var timeCasa = times[0].GetAttribute("innerText");
                var timeFora = times[1].GetAttribute("innerText");

                var dataHora = partida.FindElement(By.XPath(".//div[@class='events-list__grid__info__datetime']")).GetAttribute("innerText");

                if (dataHora.Substring(0, 2) != DateTime.Now.Day.ToString().PadLeft(2, '0') && Convert.ToInt32(dataHora.Substring(dataHora.IndexOf("\n") + 1, 2)) > 2)
                {
                    break;
                }

                Partidas.Add(new Partida(dataHora.Replace("\r\n", " "), timeCasa + " x " + timeFora));
            }

            if (Partidas.Count > 0)
            {
                var abas = Browser.FindElements(By.XPath("//ul[@class='events-tabs-container__tab']/li"));

                var abaEspeciais = abas.FirstOrDefault(x => x.GetAttribute("innerText") == "Especiais de jogadores");
                var abaAlternativas = abas.FirstOrDefault(x => x.GetAttribute("innerText") == "Linhas alternativas de jogador");

                if (abaEspeciais != null)
                {
                    UrlEspeciaisDeJogadores = "https://br.betano.com/sport/basquete/eua/nba/17106/?bt=" + abas.IndexOf(abaEspeciais);
                }

                if (abaAlternativas != null)
                {
                    UrlAlternativasDeJogadores = "https://br.betano.com/sport/basquete/eua/nba/17106/?bt=" + abas.IndexOf(abaAlternativas);
                }
            }
        }

        private void ProcurarOddsEspeciaisJogadores()
        {
            Browser.Navigate().GoToUrl(UrlEspeciaisDeJogadores);

            Browser.FindElement(By.XPath("//button[contains(@class, 'sticky-notification__cta--secondary')]")).Click();

            var menuPartidas = Browser.FindElements(By.XPath("//div[@category='BASK']"));

            foreach (var menu in menuPartidas)
            {
                var titulo = menu.FindElement(By.XPath(".//a[@class='event__header__title__link']")).GetAttribute("innerText");

                var partida = Partidas.Find(x => x.Times == titulo.Replace("-", "x"));

                if (partida == null)
                {
                    continue;
                }

                var linhas = menu.FindElements(By.XPath(".//div[@class='table-layout-container']"));

                foreach (var linha in linhas)
                {
                    var nomeLinha = linha.FindElement(By.XPath(".//span[@class='table-market-header__text']")).GetAttribute("innerText");

                    if (nomeLinha == "Double Double" || nomeLinha == "Triple Double" || nomeLinha.StartsWith("1"))
                    {
                        continue;
                    }
                    else if (nomeLinha != "Pontos Mais/Menos")
                    {
                        linha.FindElement(By.XPath(".//div[@class='table-market-header']")).Click();
                    }

                    var butMostrarTodos = linha.FindElements(By.XPath(".//button[@class='load-more']"));

                    if (butMostrarTodos.Count() > 0)
                    {
                        butMostrarTodos[0].Click();
                    }

                    var rows = linha.FindElements(By.XPath(".//div[@class='row']"));

                    foreach (IWebElement row in rows)
                    {
                        var nomeJogador = row.FindElement(By.XPath(".//div[@class='row-title']")).GetAttribute("innerText");
                        var valorLinha = Convert.ToDouble(row.FindElement(By.XPath(".//div[@class='handicap__single-item']")).GetAttribute("innerText").Replace(".", ","));
                        var oddOver = Convert.ToDouble(row.FindElement(By.XPath(".//div[@style='--selection-column-start: 1;']")).GetAttribute("innerText").Replace(".", ","));
                        var oddUnder = Convert.ToDouble(row.FindElement(By.XPath(".//div[@style='--selection-column-start: 2;']")).GetAttribute("innerText").Replace(".", ","));

                        if (!partida.Jogadores.Any(x => x.Nome == nomeJogador))
                        {
                            partida.Jogadores.Add(new Jogador(nomeJogador));
                        }

                        partida.Jogadores.Find(x => x.Nome == nomeJogador).Linhas.Add(new Linha(nomeLinha, valorLinha, oddOver, oddUnder));
                    }
                }
            }
        }

        private void ProcurarOddsAlternativas()
        {
            Browser.Navigate().GoToUrl(UrlAlternativasDeJogadores);

            var menuPartidas = Browser.FindElements(By.XPath("//div[@category='BASK']"));

            foreach (var menu in menuPartidas)
            {
                var titulo = menu.FindElement(By.XPath(".//a[@class='event__header__title__link']")).GetAttribute("innerText");

                var partida = Partidas.Find(x => x.Times == titulo.Replace("-", "x"));

                if (partida == null)
                {
                    continue;
                }

                var eleLinhas = menu.FindElements(By.XPath(".//div[@class='markets__market']"));

                foreach (IWebElement elLinha in eleLinhas)
                {
                    var aux = elLinha.FindElement(By.XPath(".//div[@class='markets__market__header__title']")).GetAttribute("innerText");
                    var nomeJogador = aux.Substring(0, aux.IndexOf(" Total"));
                    var nomeLinha = aux.Substring(aux.IndexOf(" Total") + 1);

                    if (!partida.Jogadores.Any(x => x.Nome == nomeJogador))
                    {
                        partida.Jogadores.Add(new Jogador(nomeJogador));
                    }

                    var botoes = elLinha.FindElements(By.XPath(".//button[contains(@class, 'selections__selection')]"));

                    var linhas = new List<Linha>();

                    foreach (IWebElement botao in botoes)
                    {
                        var valor = botao.FindElement(By.XPath(".//span[@class='selections__selection__title']")).GetAttribute("innerText");
                        valor = valor.Substring(0, valor.Length - 1);

                        var odd = Convert.ToDouble(botao.FindElement(By.XPath(".//span[@class='selections__selection__odd']")).GetAttribute("innerText").Replace(".", ",").Trim());

                        linhas.Add(new Linha(nomeLinha, Convert.ToDouble(valor), odd, 0));
                    }

                    partida.Jogadores.Find(x => x.Nome == nomeJogador).LinhasAlternativas.Add(new LinhaAlternativa(nomeLinha, linhas));
                }
            }
        }

        public void ProcurarHistoricoJogadores()
        {
            try
            {
                InstanciaDriver(true);

                foreach (Partida partida in Partidas)
                {
                    foreach (Jogador jogador in partida.Jogadores)
                    {
                        if (jogador.Historico != null && jogador.Historico.Partidas != null && jogador.Historico.Partidas.Count() > 0)
                        {
                            continue;
                        }

                        Browser.Navigate().GoToUrl("https://www.basketball-reference.com/search/search.fcgi?search=" + TratarNomeJogador(jogador.Nome));

                        string href = Browser.FindElement(By.XPath("//link[@rel = 'canonical']")).GetAttribute("href");

                        string url;

                        if (href.EndsWith("html"))
                        {
                            url = href.Replace(".html", "/gamelog/2023");
                        }
                        else
                        {
                            if (TemMultiplosResultados(jogador.Nome))
                            {
                                url = Browser.FindElements(By.XPath(("//div[@class = 'search-item-url']")))[1].GetAttribute("innerText").Replace(".html", "/gamelog/2023").Insert(0, "https://www.basketball-reference.com");
                            }
                            else
                            {
                                url = Browser.FindElement(By.XPath(("//div[@class = 'search-item-url']"))).GetAttribute("innerText").Replace(".html", "/gamelog/2023").Insert(0, "https://www.basketball-reference.com");
                            }
                        }

                        if (!AcessarPaginaJogador(url))
                        {
                            return;
                        }

                        if (!ProcurarStatsJogador())
                        {
                            return;
                        }

                        jogador.Historico = new HistoricoJogador();

                        JogadorStats.Reverse();

                        ProcessarStats(jogador);

                        Analisar(jogador);
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string TratarNomeJogador(string nomeJogador)
        {
            string nomeTratado;

            switch (nomeJogador)
            {
                case "Aarron Nesmith":
                    nomeTratado = "Aaron Nesmith";
                    break;
                case "Xavier Tillman Sr.":
                    nomeTratado = "Xavier Tillman";
                    break;
                default:
                    nomeTratado = nomeJogador;
                    break;
            }

            return nomeTratado.Replace(".", "").Replace("'", "").Replace("III", "").Replace("II", "").Trim().Replace(" ", "+").Replace("-", "+");
        }

        private bool TemMultiplosResultados(string nomeJogador)
        {
            if (nomeJogador == "Bogdan Bogdanovic" || nomeJogador == "Gary Payton II" || nomeJogador == "Johnny Davis")
            {
                return true;
            }

            return false;
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
            Browser.Navigate().GoToUrl(url);

            JogadorHtml.LoadHtml(Browser.PageSource);

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

            //if (!string.IsNullOrEmpty(ddlAdversario.SelectedValue))
            //{
            //    ProcessarStatsAdversario(jogador);
            //}
            //else
            //{
            //    lblPartidasContraAdv.Visible = false;
            //    lblMediasContraAdv.Visible = false;
            //    ucPartidasContraAdv.Visible = false;
            //    ucMediasContraAdv.Visible = false;
            //}

            //ProcessarDuploETriploDuplo(jogador);
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

        private void Analisar(Jogador jogador)
        {
            var lstPontos = jogador.Historico.Partidas.Select(x => x.Pontos).ToList();
            var lstRebotes = jogador.Historico.Partidas.Select(x => x.Rebotes).ToList();
            var lstAssistencias = jogador.Historico.Partidas.Select(x => x.Assistencias).ToList();
            var lstCestas3Feitas = jogador.Historico.Partidas.Select(x => x.Cestas3Feitas).ToList();
            var lstRoubos = jogador.Historico.Partidas.Select(x => x.Roubos).ToList();
            var lstBloqueios = jogador.Historico.Partidas.Select(x => x.Bloqueios).ToList();
            var lstPontosRebotesAssistencias = jogador.Historico.Partidas.Select(x => x.PontosAssistenciasRebotes).ToList();

            foreach (Linha linha in jogador.Linhas)
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
                        var lstTurnovers = jogador.Historico.Partidas.Select(x => x.InversoesPosse).ToList();
                        VerificaOcorrenciasLinha(lstTurnovers, linha);
                        break;
                    case "Pontos + Rebotes + Assistências Mais/Menos":
                        VerificaOcorrenciasLinha(lstPontosRebotesAssistencias, linha);
                        break;
                    case "Pontos + Rebotes Mais/Menos":
                        var lstPontosRebotes = jogador.Historico.Partidas.Select(x => x.PontosRebotes).ToList();
                        VerificaOcorrenciasLinha(lstPontosRebotes, linha);
                        break;
                    case "Pontos + Assistências Mais/Menos":
                        var lstPontosAssistencias = jogador.Historico.Partidas.Select(x => x.PontosAssistencias).ToList();
                        VerificaOcorrenciasLinha(lstPontosAssistencias, linha);
                        break;
                    case "Rebotes + Assistências Mais/Menos":
                        var lstRebotesAssistencias = jogador.Historico.Partidas.Select(x => x.AssistenciasRebotes).ToList();
                        VerificaOcorrenciasLinha(lstRebotesAssistencias, linha);
                        break;
                    case "Pontos + Bloqueios Mais/Menos":
                        var lstPontosBloqueios = jogador.Historico.Partidas.Select(x => x.PontosBloqueios).ToList();
                        VerificaOcorrenciasLinha(lstPontosBloqueios, linha);
                        break;
                    case "Roubadas + Bloqueios Mais/Menos":
                        var lstRoubosBloqueios = jogador.Historico.Partidas.Select(x => x.RoubosBloqueios).ToList();
                        VerificaOcorrenciasLinha(lstRoubosBloqueios, linha);
                        break;
                    default:
                        break;
                }
            }

            foreach (LinhaAlternativa linhaAlternativa in jogador.LinhasAlternativas)
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

        private void GerarPlanilha()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (File.Exists(@"C:\Users\ricardo.queiroz\Downloads\Linhas Assertivas.xls"))
            {
                File.Delete(@"C:\Users\ricardo.queiroz\Downloads\Linhas Assertivas.xls");
            }

            using (var package = new ExcelPackage(@"C:\Users\ricardo.queiroz\Downloads\Linhas Assertivas.xls"))
            {
                var sheet = package.Workbook.Worksheets.Add("PARTIDAS");

                int index = 1;

                for (int i = 0; i < Partidas.Count(); i++)
                {
                    for (int i2 = 0; i2 < Partidas[i].Jogadores.Count(); i2++)
                    {
                        for (int i3 = 0; i3 < Partidas[i].Jogadores[i2].Linhas.Count(); i3++)
                        {
                            sheet.Cells[$"A{index}"].Value = Partidas[i].Times;
                            sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                            sheet.Cells[$"C{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Nome;
                            sheet.Cells[$"D{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Valor;

                            if (Partidas[i].Jogadores[i2].Linhas[i3].SequenciaUnder > 0)
                            {
                                sheet.Cells[$"E{index}"].Value = "SIM";
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddUnder.ToString().Replace(".", ",");
                                sheet.Cells[$"G{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaUnder;
                            }
                            else
                            {
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddOver.ToString().Replace(".", ",");
                                sheet.Cells[$"G{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaOver;
                            }

                            index++;
                        }

                        for (int i3 = 0; i3 < Partidas[i].Jogadores[i2].LinhasAlternativas.Where(x => x.Linhas.Where(y => y.SequenciaOver > 0).Count() > 0).Count(); i3++)
                        {
                            var linhasSequencia = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Linhas.Where(x => x.SequenciaOver > 0).ToList();

                            for (int i4 = 0; i4 < linhasSequencia.Count(); i4++)
                            {
                                var linha = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Linhas.Find(x => x.Nome == linhasSequencia[i4].Nome && x.Valor == linhasSequencia[i4].Valor);

                                sheet.Cells[$"A{index}"].Value = Partidas[i].Times;
                                sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                                sheet.Cells[$"C{index}"].Value = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Nome;
                                sheet.Cells[$"D{index}"].Value = linha.Valor;
                                sheet.Cells[$"F{index}"].Value = linha.OddOver.ToString().Replace(".", ",");
                                sheet.Cells[$"G{index}"].Value = linha.SequenciaOver;

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

        private void EncerraProcessos()
        {
            if (Browser != null)
            {
                Browser.Close();
                Browser.Dispose();
            }

            foreach (var process in Process.GetProcessesByName("Firefox"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("geckodriver"))
            {
                process.Kill();
            }
        }
    }
}