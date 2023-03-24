using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAStats.Classes
{
    public class Linha
    {
        private string mNome;
        private string mValor;

        public string Nome
        {
            get { return mNome; }
            set { mNome = value; }
        }

        public string Valor
        {
            get { return mValor; }
            set { mValor = value; }
        }

        public Linha(string pNome, string pValor)
        {
            mNome = pNome;
            mValor = pValor;
        }
    }
}