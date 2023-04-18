using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBAStats.Classes
{
    public class PartidaJogador
    {
        private string mData;
        public string auxAdversario { get; set; }
        public string auxCestas2 { get; set; }
        public string auxCestas3 { get; set; }
        public string auxLancesLivres { get; set; }
        public string Time { get; set; }
        public string Resultado { get; set; }
        public int Minutos { get; set; }        
        public int Rebotes { get; set; }
        public int Assistencias { get; set; }
        public int Bloqueios { get; set; }
        public int Roubos { get; set; }
        public int Faltas { get; set; }
        public int InversoesPosse { get; set; }
        public int Pontos { get; set; }

        public string Adversario
        {
            get { return auxAdversario.Replace("vs", "").Replace("@", ""); }
        }

        public bool EmCasa
        {
            get { return auxAdversario.StartsWith("vs"); }
        }

        public string Data
        {
            get { return Convert.ToDateTime(new DateTime(DateTime.Now.Year, Convert.ToInt32(mData.Substring(0, mData.IndexOf('/'))), Convert.ToInt32(mData.Substring(mData.IndexOf('/') + 1)))).ToString("dd/MM/yyyy"); }
            set { mData = value; }
        }

        public int Cestas2
        {
            get { return (Convert.ToInt32(auxCestas2.Substring(0, auxCestas2.IndexOf('-'))) - Convert.ToInt32(auxCestas3.Substring(0, auxCestas3.IndexOf('-')))); }
        }

        public int Cestas2Tentativas
        {
            get { return (Convert.ToInt32(auxCestas2.Substring(auxCestas2.IndexOf('-') + 1)) - Convert.ToInt32(auxCestas3.Substring(auxCestas3.IndexOf('-') + 1))); }
        }

        public int Cestas3
        {
            get { return Convert.ToInt32(auxCestas3.Substring(0, auxCestas3.IndexOf('-'))); }
        }

        public int Cestas3Tentativas
        {
            get { return Convert.ToInt32(auxCestas3.Substring(auxCestas3.IndexOf('-') + 1)); }
        }

        public int LancesLivres
        {
            get { return Convert.ToInt32(auxLancesLivres.Substring(0, auxLancesLivres.IndexOf('-'))); }
        }

        public int LancesLivresTentativas
        {
            get { return Convert.ToInt32(auxLancesLivres.Substring(auxLancesLivres.IndexOf('-') + 1)); }
        }

        public bool DuploDuplo
        {
            get
            {
                return DuploTriploDuplo >= 2;
            }
        }

        public bool TriploDuplo
        {
            get
            {
                return DuploTriploDuplo >= 3;
            }
        }

        private int DuploTriploDuplo
        {
            get
            {
                int aux = 0;

                if (Pontos >= 10)
                {
                    aux++;
                }

                if (Assistencias >= 10)
                {
                    aux++;
                }

                if (Rebotes >= 10)
                {
                    aux++;
                }

                if (Bloqueios >= 10)
                {
                    aux++;
                }

                if (Roubos >= 10)
                {
                    aux++;
                }

                return aux;
            }
        }

        public int PontosAssistenciasRebotes
        {
            get { return Pontos + Assistencias + Rebotes; }
        }

        public int PontosAssistencias
        {
            get { return Pontos + Assistencias; }
        }

        public int PontosRebotes
        {
            get { return Pontos + Rebotes; }
        }

        public int AssistenciasRebotes
        {
            get { return Assistencias + Rebotes; }
        }

        public int RoubosBloqueios
        {
            get { return Roubos + Bloqueios; }
        }

        public int PontosBloqueios
        {
            get { return Pontos + Bloqueios; }
        }

        public static PartidaJogador DictionaryDePara(Dictionary<string, string> dic)
        {
            var partida = new PartidaJogador();

            foreach (var key in dic.Keys)
            {
                PropertyInfo property = partida.GetType().GetProperty(key);
                property.SetValue(partida, Convert.ChangeType(dic[key], property.PropertyType), null);
            }

            return partida;
        }
    }
}