using System;
using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class Partida
    {
        public DateTime DataHora;
        public string Times;
        public List<Jogador> Jogadores;
        public string Url;
        public List<LinhaAssertiva> LinhasAssertivas;

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
            LinhasAssertivas = new List<LinhaAssertiva>();
        }
    }
}