using System;
using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class Partida
    {
        public DateTime DataHora { get; set; }
        public string TimeCasa { get; set; }
        public string TimeFora { get; set; }
        public string Times { get { return TimeCasa + " x " + TimeFora; } }
        public List<Jogador> Jogadores { get; set; }

        public Partida(string pDataHora, string pTimeCasa, string pTimeFora)
        {
            var mes = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf("/") + 1, 2));
            var dia = Convert.ToInt32(pDataHora.Substring(0, pDataHora.IndexOf("/")));
            var hora = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf(" ") + 1, 2));
            var minuto = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf(":") + 1));

            DataHora = new DateTime(DateTime.Now.Year, mes, dia, hora, minuto, 0);
            TimeCasa = pTimeCasa;
            TimeFora = pTimeFora;
            Jogadores = new List<Jogador>();
        }
    }
}