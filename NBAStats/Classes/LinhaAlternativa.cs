using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class LinhaAlternativa
    {
        public string Nome { get; set; }
        public List<Linha> Linhas { get; set; }

        public LinhaAlternativa(string pNome, List<Linha> pLinhas)
        {
            Nome = pNome;
            Linhas = pLinhas;
        }
    }
}