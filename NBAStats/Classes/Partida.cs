using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBAStats.Classes
{
    public class Partida
    {
        private DateTime mDataHora;
        private string mTimes;
        private List<Jogador> mJogadores;
        private string mUrl;
        private List<LinhaAssertiva> mLinhasAssertivas;

        public DateTime DataHora
        {
            get { return mDataHora; }
            set { mDataHora = value; }
        }

        public string Times
        {
            get { return mTimes; }
            set { mTimes = value; }
        }

        public List<Jogador> Jogadores
        {
            get { return mJogadores; }
            set { mJogadores = value; }
        }

        public string Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

        public List<LinhaAssertiva> LinhasAssertivas
        {
            get { return mLinhasAssertivas; }
            set { mLinhasAssertivas = value; }
        }

        public Partida(string pDataHora, string pTimes, string pUrl)
        {
            var mes = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf("/") + 1, 2));
            var dia = Convert.ToInt32(pDataHora.Substring(0, pDataHora.IndexOf("/")));
            var hora = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf(" ") + 1, 2));
            var minuto = Convert.ToInt32(pDataHora.Substring(pDataHora.IndexOf(":") + 1));

            mDataHora = new DateTime(DateTime.Now.Year, mes, dia, hora, minuto, 0);
            mTimes = pTimes;
            mJogadores = new List<Jogador>();
            mUrl = pUrl;
            mLinhasAssertivas = new List<LinhaAssertiva>();
        }
    }
}