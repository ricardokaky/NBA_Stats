namespace NBAStats.Classes
{
    public class Linha
    {
        public string Nome;
        public double Valor;
        public double OddOver;
        public double OddUnder;

        public Linha(string pNome, double pValor, double pOddOver, double pOddUnder)
        {
            Nome = pNome;
            Valor = pValor;
            OddOver = pOddOver;
            OddUnder = pOddUnder;
        }
    }
}