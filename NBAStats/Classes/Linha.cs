namespace NBAStats.Classes
{
    public class Linha
    {
        public string Nome { get; set; }
        public double Valor { get; set; }
        public double OddOver { get; set; }
        public double OddUnder { get; set; }
        public int SequenciaOver { get; set; }
        public int SequenciaUnder { get; set; }

        public Linha(string pNome, double pValor, double pOddOver, double pOddUnder)
        {
            Nome = pNome;
            Valor = pValor;
            OddOver = pOddOver;
            OddUnder = pOddUnder;
        }
    }
}