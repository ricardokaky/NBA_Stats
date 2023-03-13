using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBAStats.Classes
{
    public class Partida
    {
        private string mDataHora;
        private string mTimes;
        private List<Jogador> mJogadores;

        //public DateTime DataHora
        //{
        //    get { return mDataHora; }
        //    set { mDataHora = value; }
        //}

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

        public Partida(string pDataHora, string pTimes)
        {
            mDataHora = pDataHora;
            mTimes = pTimes;
        }
    }
}