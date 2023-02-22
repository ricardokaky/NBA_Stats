using System.Collections.Generic;

namespace NBAStats
{
    public class Jogador
    {
        public string Nome { get; set; }
        public List<Partida> Partidas { get; set; }
        public List<Partida> PartidasContraAdv { get; set; }
        public List<Media> Medias { get; set; }
        public List<Media> MediasContraAdv { get; set; }

        public Jogador(string pNome)
        {
            Nome = pNome;
            Partidas = new List<Partida>();
            Medias = new List<Media>()
            {
                new Media("Cestas3Feitas", 0, 0),
                new Media("Rebotes", 0, 0),
                new Media("Assistencias", 0, 0),
                new Media("Bloqueios", 0, 0),
                new Media("Roubos", 0, 0),
                new Media("InversoesPosse", 0, 0),
                new Media("Pontos", 0, 0),
                new Media("PontosAssistenciasRebotes", 0, 0),
                new Media("PontosAssistencias", 0, 0),
                new Media("PontosRebotes", 0, 0),
                new Media("AssistenciasRebotes", 0, 0),
                new Media("RoubosBloqueios", 0, 0)
            };
            PartidasContraAdv = new List<Partida>();
            MediasContraAdv = new List<Media>()
            {
                new Media("Cestas3Feitas", 0, 0),
                new Media("Rebotes", 0, 0),
                new Media("Assistencias", 0, 0),
                new Media("Bloqueios", 0, 0),
                new Media("Roubos", 0, 0),
                new Media("InversoesPosse", 0, 0),
                new Media("Pontos", 0, 0),
                new Media("PontosAssistenciasRebotes", 0, 0),
                new Media("PontosAssistencias", 0, 0),
                new Media("PontosRebotes", 0, 0),
                new Media("AssistenciasRebotes", 0, 0),
                new Media("RoubosBloqueios", 0, 0)
            };
        }
    }
}