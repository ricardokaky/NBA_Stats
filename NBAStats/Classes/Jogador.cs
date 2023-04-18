using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class Jogador
    {
        public string Nome { get; set; }
        public List<Linha> Linhas { get; set; }
        public List<LinhaAlternativa> LinhasAlternativas { get; set; }
        public HistoricoJogador Historico { get; set; }
        public string Time { get; set; }

        public Jogador(string pNome)
        {
            Nome = pNome;
            Linhas = new List<Linha>();
            LinhasAlternativas = new List<LinhaAlternativa>();
        }
    }
}