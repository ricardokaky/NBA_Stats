using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class Jogador
    {
        private string mNome { get; set; }
        public List<Linha> Linhas { get; set; }

        public string Nome
        {
            get { return mNome; }
            set { mNome = value; }
        }

        public Jogador(string pNome)
        {
            mNome = pNome;
        }
    }
}