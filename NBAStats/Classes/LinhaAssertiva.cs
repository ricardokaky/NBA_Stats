using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAStats.Classes
{
    public class LinhaAssertiva
    {
        public string NomeJogador;
        public Linha Linha;
        public bool Over;
        public double Odd;

        public LinhaAssertiva(string pNomeJogador, Linha pLinha, bool pOver = true)
        {
            NomeJogador = pNomeJogador;
            Linha = pLinha;
            Over = pOver;
            Odd = Over ? Linha.OddOver : Linha.OddUnder;
        }
    }
}