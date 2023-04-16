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

            var partidasDisponiveis = Browser.FindElements(By.XPath("//div[@class='events-list__grid__event']"));

            Partidas = new List<Partida>();

            foreach (var partida in partidasDisponiveis)
            {
                var hyperlink = partida.FindElement(By.XPath(".//div[@class='events-list__grid__info__main__row']"));

                if (hyperlink.GetAttribute("innerText").Contains("AO VIVO"))
                {
                    continue;
                }

                var times = hyperlink.FindElements(By.XPath(".//span[contains(@class, 'events-list__grid__info__main__participants__participant-name')]"));
                var timeCasa = times[0].GetAttribute("innerText");
                var timeFora = times[1].GetAttribute("innerText");

                var dataHora = partida.FindElement(By.XPath(".//div[@class='events-list__grid__info__datetime']")).GetAttribute("innerText");

                if (dataHora.Substring(0, 2) != DateTime.Now.Day.ToString().PadLeft(2, '0') && Convert.ToInt32(dataHora.Substring(dataHora.IndexOf("\n") + 1, 2)) > 2)
                {
                    break;
                }

                if (timeCasa == "Los Angeles Clippers")
                {
                    timeCasa = "LA Clippers";
                }
                else if (timeFora == "Los Angeles Clippers")
                {
                    timeFora = "LA Clippers";
                }

                Partidas.Add(new Partida(dataHora.Replace("\r\n", " "), timeCasa, timeFora));
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

                    if (nomeLinha.StartsWith("1") || nomeLinha == "Faltas recebidas Mais/Menos" || nomeLinha == "Expulsão por faltas")
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
                        double valorLinha = 0;

                        if (nomeLinha != "Double Double" && nomeLinha != "Triple Double")
                        {
                            valorLinha = Convert.ToDouble(row.FindElement(By.XPath(".//div[@class='handicap__single-item']")).GetAttribute("innerText").Replace(".", ","));
                        }

                        var nomeJogador = row.FindElement(By.XPath(".//div[@class='row-title']")).GetAttribute("innerText");
                        double oddOver = 0;
                        double oddUnder = 0;
                        var auxOddOver = row.FindElements(By.XPath(".//div[@style='--selection-column-start: 1;']"));
                        var auxOddUnder = row.FindElements(By.XPath(".//div[@style='--selection-column-start: 2;']"));

                        if (auxOddOver.Count() > 0)
                        {
                            oddOver = Convert.ToDouble(auxOddOver[0].GetAttribute("innerText").Replace(".", ","));
                        }

                        if (auxOddUnder.Count() > 0)
                        {
                            oddUnder = Convert.ToDouble(auxOddUnder[0].GetAttribute("innerText").Replace(".", ","));
                        }

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
                InstanciaDriver(false);

                foreach (Partida partida in Partidas)
                {
                    foreach (Jogador jogador in partida.Jogadores)
                    {
                        if (jogador.Historico != null && jogador.Historico.Partidas != null && jogador.Historico.Partidas.Count() > 0)
                        {
                            continue;
                        }

                        Browser.Navigate().GoToUrl("https://www.espn.com/search/_/type/players/q/" + TratarNomeJogador(jogador.Nome));

                        string href = Browser.FindElement(By.XPath("//a[contains(@class, 'LogoTile')]")).GetAttribute("href");

                        string url = href.Insert(href.IndexOf("_/"), "gamelog/");

                        if (!AcessarPaginaJogador(url))
                        {
                            return;
                        }

                        if (!ProcurarStatsJogador())
                        {
                            return;
                        }

                        jogador.Time = Browser.FindElement(By.XPath("//a[@class='AnchorLink clr-black']")).GetAttribute("innerText");

                        ProcessarStats(jogador);

                        Analisar(partida, jogador);
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
                case "DeAndre Hunter":
                    nomeTratado = "De'Andre Hunter";
                    break;
                case "Nicolas Claxton":
                    nomeTratado = "Nic Claxton";
                    break;
                case "Royce ONeale":
                    nomeTratado = "Royce O'Neale";
                    break;
                case "DeAnthony Melton":
                    nomeTratado = "De'Anthony Melton";
                    break;
                case "DeAaron Fox":
                    nomeTratado = "De'Aaron Fox";
                    break;
                default:
                    nomeTratado = nomeJogador;
                    break;
            }

            return nomeTratado.Replace(" ", "%20").Replace("-", "%20").Trim();
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
            JogadorStats = new List<HtmlNode>(JogadorHtml.DocumentNode.SelectNodes("//div[@class='mb5']//tr[@class='Table__TR Table__TR--sm Table__even' or @class='filled Table__TR Table__TR--sm Table__even']").ToList());

            return JogadorStats.Count > 0;
        }

        private void ProcessarStats(Jogador jogador)
        {
            jogador.Historico = new HistoricoJogador();
            jogador.Historico.Partidas.Clear();
            jogador.Historico.PartidasContraAdv.Clear();

            foreach (HtmlNode node in JogadorStats)
            {
                var lstNodes = node.ChildNodes.Where(x => x.Name == "td").ToList();

                lstNodes.RemoveAt(5);
                lstNodes.RemoveAt(6);
                lstNodes.RemoveAt(7);

                jogador.Historico.Partidas.Add(ProcessarPartida(lstNodes));
            }
        }

        private PartidaJogador ProcessarPartida(List<HtmlNode> nodes)
        {
            var dicStats = new Dictionary<string, string>()
            {
                { "Data", null },
                { "auxAdversario", null },
                { "Resultado", null },
                { "Minutos", null },
                { "auxCestas2", null },
                { "auxCestas3", null },
                { "auxLancesLivres", null },
                { "Rebotes", null },
                { "Assistencias", null },
                { "Bloqueios", null },
                { "Roubos", null },
                { "Faltas", null },
                { "InversoesPosse", null },
                { "Pontos", null }
            };

            for (int i = 0; i < nodes.Count(); i++)
            {
                string text = nodes[i].InnerText;

                if (i == 0)
                {
                    text = text.Substring(text.IndexOf(" ") + 1);
                }
                else if (i == 2)
                {
                    text = text.Insert(1, " ");
                }

                dicStats[dicStats.ElementAt(i).Key] = text;
            }

            return PartidaJogador.DictionaryDePara(dicStats);
        }

        private void Analisar(Partida partida, Jogador jogador)
        {
            string adversario = "";

            if (partida.TimeCasa == jogador.Time)
            {
                adversario = SiglaTime(partida.TimeFora);
            }
            else
            {
                adversario = SiglaTime(partida.TimeCasa);
            }

            var jaJogouContra = jogador.Historico.Partidas.Where(x => x.Adversario == adversario).Count() > 0;

            var EmCasa = partida.TimeCasa == jogador.Time;

            var jaJogouEmCasaOuFora = jogador.Historico.Partidas.Where(x => x.EmCasa == EmCasa).Count() > 0;

            foreach (Linha linha in jogador.Linhas)
            {
                if (linha.Nome != "Double Double" && linha.Nome != "Triple Double")
                {
                    var lst = jogador.Historico.Partidas.Select(x => Convert.ToInt32(x.GetType().GetProperty(PropriedadeLinha(linha.Nome)).GetValue(x))).ToList();

                    VerificaOcorrenciasLinha(lst, linha);

                    linha.MediaTemporada = lst.Average();

                    if (jaJogouContra)
                    {
                        linha.MediaAdversario = jogador.Historico.Partidas.Where(x => x.Adversario == adversario).Select(x => Convert.ToInt32(x.GetType().GetProperty(PropriedadeLinha(linha.Nome)).GetValue(x))).ToList().Average();
                    }

                    if (jaJogouEmCasaOuFora)
                    {
                        linha.MediaCasaOuFora = jogador.Historico.Partidas.Where(x => x.EmCasa == EmCasa).Select(x => Convert.ToInt32(x.GetType().GetProperty(PropriedadeLinha(linha.Nome)).GetValue(x))).ToList().Average();
                    }

                    linha.Percent5PartidasOver = ((double)lst.Take(5).Where(x => x > linha.Valor).Count() / 5) * 100;
                    linha.Percent5PartidasUnder = ((double)lst.Take(5).Where(x => x < linha.Valor).Count() / 5) * 100;

                    linha.Percent10PartidasOver = ((double)lst.Take(10).Where(x => x > linha.Valor).Count() / 10) * 100;
                    linha.Percent10PartidasUnder = ((double)lst.Take(10).Where(x => x < linha.Valor).Count() / 10) * 100;

                    linha.PercentTemporadaOver = ((double)lst.Where(x => x > linha.Valor).Count() / jogador.Historico.Partidas.Count()) * 100;
                    linha.PercentTemporadaUnder = ((double)lst.Where(x => x < linha.Valor).Count() / jogador.Historico.Partidas.Count()) * 100;
                }
                else
                {
                    var lst = jogador.Historico.Partidas.Select(x => Convert.ToBoolean(x.GetType().GetProperty(PropriedadeLinha(linha.Nome)).GetValue(x))).ToList();
                    VerificaOcorrenciasDuploETriplo(lst, linha);

                    if (jogador.Historico.Partidas.Count() >= 5)
                    {
                        linha.Percent5PartidasOver = ((double)lst.Take(5).Where(x => x).Count() / 5) * 100;
                        linha.Percent5PartidasUnder = ((double)lst.Take(5).Where(x => !x).Count() / 5) * 100;
                    }

                    if (jogador.Historico.Partidas.Count() >= 10)
                    {
                        linha.Percent10PartidasOver = ((double)lst.Take(10).Where(x => x).Count() / 10) * 100;
                        linha.Percent10PartidasUnder = ((double)lst.Take(10).Where(x => !x).Count() / 10) * 100;
                    }

                    linha.PercentTemporadaOver = ((double)lst.Where(x => x).Count() / jogador.Historico.Partidas.Count()) * 100;
                    linha.PercentTemporadaUnder = ((double)lst.Where(x => !x).Count() / jogador.Historico.Partidas.Count()) * 100;
                }
            }

            foreach (LinhaAlternativa linhaAlternativa in jogador.LinhasAlternativas)
            {
                var lst = jogador.Historico.Partidas.Select(x => Convert.ToInt32(x.GetType().GetProperty(PropriedadeLinha(linhaAlternativa.Nome)).GetValue(x))).ToList();

                VerificaOcorrenciasLinhaAlternativa(lst, linhaAlternativa.Linhas);

                linhaAlternativa.Linhas.ForEach(x => x.MediaTemporada = lst.Average());

                if (jaJogouContra)
                {
                    linhaAlternativa.Linhas.ForEach(x => x.MediaAdversario = jogador.Historico.Partidas.Where(y => y.Adversario == adversario).Select(y => Convert.ToInt32(y.GetType().GetProperty(PropriedadeLinha(linhaAlternativa.Nome)).GetValue(y))).ToList().Average());
                }

                if (jaJogouEmCasaOuFora)
                {
                    linhaAlternativa.Linhas.ForEach(x => x.MediaCasaOuFora = jogador.Historico.Partidas.Where(y => y.EmCasa == EmCasa).Select(y => Convert.ToInt32(y.GetType().GetProperty(PropriedadeLinha(linhaAlternativa.Nome)).GetValue(y))).ToList().Average());
                }

                if (jogador.Historico.Partidas.Count() >= 5)
                {
                    linhaAlternativa.Linhas.ForEach(x => x.Percent5PartidasOver = ((double)lst.Take(5).Where(y => y > x.Valor).Count() / 5) * 100);
                }

                if (jogador.Historico.Partidas.Count() >= 10)
                {
                    linhaAlternativa.Linhas.ForEach(x => x.Percent10PartidasOver = ((double)lst.Take(10).Where(y => y > x.Valor).Count() / 10) * 100);
                }

                linhaAlternativa.Linhas.ForEach(x => x.PercentTemporadaOver = ((double)lst.Where(y => y > x.Valor).Count() / jogador.Historico.Partidas.Count()) * 100);
            }
        }

        private string PropriedadeLinha(string nomeLinha)
        {
            string propriedade = "";

            switch (nomeLinha)
            {
                case "Pontos Mais/Menos":
                case "Total de Pontos":
                    propriedade = "Pontos";
                    break;
                case "Rebotes Mais/Menos":
                case "Total de Rebotes":
                    propriedade = "Rebotes";
                    break;
                case "Assistências Mais/Menos":
                case "Total de Assistências":
                    propriedade = "Assistencias";
                    break;
                case "Total Arremessos de três pontos Marcados +/-":
                case "Total Arremessos de três pontos Marcados":
                    propriedade = "Cestas3";
                    break;
                case "Total de 2 pontos marcados mais/menos":
                    propriedade = "Cestas2";
                    break;
                case "Roubos Mais/Menos":
                case "Total de Roubos":
                    propriedade = "Roubos";
                    break;
                case "Bloqueios Mais/Menos":
                case "Total de tocos":
                    propriedade = "Bloqueios";
                    break;
                case "Turnover Mais/Menos":
                    propriedade = "InversoesPosse";
                    break;
                case "Pontos + Rebotes + Assistências Mais/Menos":
                case "Total de Pontos, Rebotes e Assistências":
                    propriedade = "PontosAssistenciasRebotes";
                    break;
                case "Pontos + Rebotes Mais/Menos":
                    propriedade = "PontosRebotes";
                    break;
                case "Pontos + Assistências Mais/Menos":
                    propriedade = "PontosAssistencias";
                    break;
                case "Rebotes + Assistências Mais/Menos":
                    propriedade = "AssistenciasRebotes";
                    break;
                case "Pontos + Bloqueios Mais/Menos":
                    propriedade = "PontosBloqueios";
                    break;
                case "Roubadas + Bloqueios Mais/Menos":
                    propriedade = "RoubosBloqueios";
                    break;
                case "Faltas cometidas Mais/Menos":
                    propriedade = "Faltas";
                    break;
                case "Tentativas de lançamentos de três pontos mais/menos":
                    propriedade = "Cestas3Tentativas";
                    break;
                case "Tentativas de lançamentos de dois pontos mais/menos":
                    propriedade = "Cestas2Tentativas";
                    break;
                case "Lances livres marcados Mais/Menos":
                    propriedade = "LancesLivres";
                    break;
                case "Total de Lances livres Mais/Menos":
                    propriedade = "LancesLivresTentativas";
                    break;
                case "Tempo de jogo dos jogadores mais/menos":
                    propriedade = "Minutos";
                    break;
                case "Double Double":
                    propriedade = "DuploDuplo";
                    break;
                case "Triple Double":
                    propriedade = "TriploDuplo";
                    break;
                default:
                    break;
            }

            return propriedade;
        }

        private string SiglaTime(string time)
        {
            string sigla = "";

            switch (time.ToUpper())
            {
                case "DALLAS MAVERICKS":
                    sigla = "DAL";
                    break;
                case "UTAH JAZZ":
                    sigla = "UTAH";
                    break;
                case "PHOENIX SUNS":
                    sigla = "PHX";
                    break;
                case "LA CLIPPERS":
                    sigla = "LAC";
                    break;
                case "HOUSTON ROCKETS":
                    sigla = "HOU";
                    break;
                case "MINNESOTA TIMBERWOLVES":
                    sigla = "MIN";
                    break;
                case "CHICAGO BULLS":
                    sigla = "CHI";
                    break;
                case "GOLDEN STATE WARRIORS":
                    sigla = "GS";
                    break;
                case "NEW ORLEANS PELICANS":
                    sigla = "NO";
                    break;
                case "INDIANA PACERS":
                    sigla = "IND";
                    break;
                case "NEW YORK KNICKS":
                    sigla = "NY";
                    break;
                case "BOSTON CELTICS":
                    sigla = "BOS";
                    break;
                case "SAN ANTONIO SPURS":
                    sigla = "SA";
                    break;
                case "PORTLAND TRAIL BLAZERS":
                    sigla = "BOS";
                    break;
                case "MEMPHIS GRIZZLIES":
                    sigla = "MEM";
                    break;
                case "SACRAMENTO KINGS":
                    sigla = "SAC";
                    break;
                case "PHILADELPHIA 76ERS":
                    sigla = "PHI";
                    break;
                case "ATLANTA HAWKS":
                    sigla = "ATL";
                    break;
                case "CHARLOTTE HORNETS":
                    sigla = "CHA";
                    break;
                case "MIAMI HEAT":
                    sigla = "MIA";
                    break;
                case "ORLANDO MAGIC":
                    sigla = "ORL";
                    break;
                case "WASHINGTON WIZARDS":
                    sigla = "WSH";
                    break;
                case "DENVER NUGGETS":
                    sigla = "DEN";
                    break;
                case "DETROIT PISTONS":
                    sigla = "DET";
                    break;
                case "CLEVELAND CAVALIERS":
                    sigla = "CLE";
                    break;
                case "MILWAUKEE BUCKS":
                    sigla = "MIL";
                    break;
                case "BROOKLYN NETS":
                    sigla = "BKN";
                    break;
                case "TORONTO RAPTORS":
                    sigla = "TOR";
                    break;
                case "OKLAHOMA CITY THUNDER":
                    sigla = "OKC";
                    break;
                case "LOS ANGELES LAKERS":
                    sigla = "LAL";
                    break;
                default:
                    break;
            }

            return sigla;
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

        private void VerificaOcorrenciasDuploETriplo(List<bool> lista, Linha linha)
        {
            if (lista[0])
            {
                if (lista.Any(x => !x))
                {
                    linha.SequenciaOver = lista.IndexOf(lista.First(x => !x));
                }
                else
                {
                    linha.SequenciaOver = lista.Count();
                }
            }
            else
            {
                if (lista.Any(x => x))
                {
                    linha.SequenciaUnder = lista.IndexOf(lista.First(x => x));
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
                            if ((Partidas[i].Jogadores[i2].Linhas[i3].SequenciaUnder > 0 && Partidas[i].Jogadores[i2].Linhas[i3].OddUnder == 0) ||
                                (Partidas[i].Jogadores[i2].Linhas[i3].SequenciaOver > 0 && Partidas[i].Jogadores[i2].Linhas[i3].OddOver == 0))
                            {
                                continue;
                            }

                            var nomeLinha = Partidas[i].Jogadores[i2].Linhas[i3].Nome;

                            if (nomeLinha != "Double Double" && nomeLinha != "Triple Double")
                            {
                                string mediaAdversario = "";
                                string mediaCasaOuFora = "";

                                if (Partidas[i].Jogadores[i2].Linhas[i3].MediaAdversario != null)
                                {
                                    mediaAdversario = Convert.ToDouble(Partidas[i].Jogadores[i2].Linhas[i3].MediaAdversario).ToString("0.00");
                                }

                                if (Partidas[i].Jogadores[i2].Linhas[i3].MediaCasaOuFora != null)
                                {
                                    mediaCasaOuFora = Convert.ToDouble(Partidas[i].Jogadores[i2].Linhas[i3].MediaCasaOuFora).ToString("0.00");
                                }

                                sheet.Cells[$"A{index}"].Value = Partidas[i].Times;
                                sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                                sheet.Cells[$"C{index}"].Value = nomeLinha;
                                sheet.Cells[$"D{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Valor;
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddOver.ToString().Replace(".", ",");
                                sheet.Cells[$"G{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].MediaTemporada.ToString("0.00");
                                sheet.Cells[$"H{index}"].Value = mediaAdversario;
                                sheet.Cells[$"I{index}"].Value = mediaCasaOuFora;
                                sheet.Cells[$"J{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent5PartidasOver.ToString() + "%";
                                sheet.Cells[$"K{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent10PartidasOver.ToString() + "%";
                                sheet.Cells[$"L{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].PercentTemporadaOver.ToString("0.00") + "%";
                                sheet.Cells[$"M{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaOver;

                                index++;

                                sheet.Cells[$"A{index}"].Value = Partidas[i].Times;
                                sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                                sheet.Cells[$"C{index}"].Value = nomeLinha;
                                sheet.Cells[$"D{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Valor;
                                sheet.Cells[$"E{index}"].Value = "SIM";
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddUnder.ToString().Replace(".", ",");
                                sheet.Cells[$"G{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].MediaTemporada.ToString("0.00");
                                sheet.Cells[$"H{index}"].Value = mediaAdversario;
                                sheet.Cells[$"I{index}"].Value = mediaCasaOuFora;
                                sheet.Cells[$"J{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent5PartidasUnder.ToString() + "%";
                                sheet.Cells[$"K{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent10PartidasUnder.ToString() + "%";
                                sheet.Cells[$"L{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].PercentTemporadaUnder.ToString("0.00") + "%";
                                sheet.Cells[$"M{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaUnder;
                            }
                            else
                            {
                                sheet.Cells[$"A{index}"].Value = Partidas[i].Times;
                                sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                                sheet.Cells[$"C{index}"].Value = nomeLinha;
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddOver.ToString().Replace(".", ",");
                                sheet.Cells[$"J{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent5PartidasOver.ToString() + "%";
                                sheet.Cells[$"K{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent10PartidasOver.ToString() + "%";
                                sheet.Cells[$"L{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].PercentTemporadaOver.ToString("0.00") + "%";
                                sheet.Cells[$"M{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaOver;

                                index++;

                                sheet.Cells[$"A{index}"].Value = Partidas[i].Times;
                                sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                                sheet.Cells[$"C{index}"].Value = nomeLinha;
                                sheet.Cells[$"E{index}"].Value = "SIM";
                                sheet.Cells[$"F{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].OddUnder.ToString().Replace(".", ",");
                                sheet.Cells[$"J{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent5PartidasUnder.ToString() + "%";
                                sheet.Cells[$"K{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].Percent10PartidasUnder.ToString() + "%";
                                sheet.Cells[$"L{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].PercentTemporadaUnder.ToString("0.00") + "%";
                                sheet.Cells[$"M{index}"].Value = Partidas[i].Jogadores[i2].Linhas[i3].SequenciaUnder;
                            }

                            index++;
                        }

                        for (int i3 = 0; i3 < Partidas[i].Jogadores[i2].LinhasAlternativas.Count(); i3++)
                        {
                            var linhasSequencia = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Linhas.ToList();

                            for (int i4 = 0; i4 < linhasSequencia.Count(); i4++)
                            {
                                var linha = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Linhas.Find(x => x.Nome == linhasSequencia[i4].Nome && x.Valor == linhasSequencia[i4].Valor);

                                sheet.Cells[$"A{index}"].Value = Partidas[i].Times;
                                sheet.Cells[$"B{index}"].Value = Partidas[i].Jogadores[i2].Nome;
                                sheet.Cells[$"C{index}"].Value = Partidas[i].Jogadores[i2].LinhasAlternativas[i3].Nome;
                                sheet.Cells[$"D{index}"].Value = linha.Valor;
                                sheet.Cells[$"F{index}"].Value = linha.OddOver.ToString().Replace(".", ",");
                                sheet.Cells[$"G{index}"].Value = linha.MediaTemporada.ToString("{0.00}");

                                if (linha.MediaAdversario != null)
                                {
                                    sheet.Cells[$"H{index}"].Value = Convert.ToDouble(linha.MediaAdversario).ToString("0.00");
                                }

                                if (linha.MediaCasaOuFora != null)
                                {
                                    sheet.Cells[$"I{index}"].Value = Convert.ToDouble(linha.MediaCasaOuFora).ToString("0.00");
                                }

                                sheet.Cells[$"J{index}"].Value = linha.Percent5PartidasOver.ToString() + "%";
                                sheet.Cells[$"K{index}"].Value = linha.Percent10PartidasOver.ToString() + "%";
                                sheet.Cells[$"L{index}"].Value = linha.PercentTemporadaOver.ToString("0.00") + "%";
                                sheet.Cells[$"M{index}"].Value = linha.SequenciaOver;

                                index++;
                            }
                        }
                    }
                }

                package.Save();
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