using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAStats.Classes
{
    public class LinhaAssertiva
    {
        private string mNomeJogador;
        private Linha mLinha;
        private bool mOver;

        public Linha Linha
        {
            get { return mLinha; }
            set { mLinha = value; }
        }

        public string NomeJogador
        {
            get { return mNomeJogador; }
            set { mNomeJogador = value; }
        }

        public bool Over
        {
            get { return mOver; }
            set { mOver = value; }
        }

        public LinhaAssertiva(string pNomeJogador, string pNomeLinha, double pValor, bool pOver = true)
        {
            mNomeJogador = pNomeJogador;
            mLinha = new Linha(pNomeLinha, pValor.ToString());
            mOver = pOver;
        }
    }
}