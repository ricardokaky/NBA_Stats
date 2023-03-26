using System;
using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class Partida
    {
        public DateTime DataHora { get; set; }
        public string Times { get; set; }
        public List<Jogador> Jogadores { get; set; }
        public string Url { get; set; }

        public Partida(string pDataHora, string pTimes, string pUrl)
        {
            var mes = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf("/") + 1, 2));
            var dia = Convert.ToInt32(pDataHora.Substring(0, pDataHora.IndexOf("/")));
            var hora = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf(" ") + 1, 2));
            var minuto = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf(":") + 1));

            DataHora = new DateTime(DateTime.Now.Year, mes, dia, hora, minuto, 0);
            Times = pTimes;
            Jogadores = new List<Jogador>();
            Url = pUrl;
        }
    }
}