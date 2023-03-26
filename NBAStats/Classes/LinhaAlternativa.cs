using System.Collections.Generic;

namespace NBAStats.Classes
{
    public class LinhaAlternativa
    {
        public string Nome;
        public List<Linha> Linhas;

        public LinhaAlternativa(string pNome, List<Linha> pLinhas)
        {
            Nome = pNome;
            Linhas = pLinhas;
        }
    }
}