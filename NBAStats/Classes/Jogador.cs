using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class Jogador
    {
        private string mNome { get; set; }
        public Dictionary<string, double> Linhas { get; set; }
        public Dictionary<string, List<int>> LinhasAlternativas { get; set; }

        public string Nome
        {
            get { return mNome; }
            set { mNome = value; }
        }

        public Jogador(string pNome)
        {
            mNome = pNome;
            Linhas = new Dictionary<string, double>();
            LinhasAlternativas = new Dictionary<string, List<int>>();
        }
    }
}