using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBAStats
{
    public class Partida
    {
        private string mData;
        private string mTime;
        private string mAdversario;
        private string mResultado;
        private string mMinutos;
        private int mFieldGoalsFeitos;
        private int mFieldGoalsTentados;
        private int mCestas3Feitas;
        private int mCestas3Tentadas;
        private double mCestas3Percentual;
        private int mLancesLivresFeitos;
        private int mLancesLivresTentados;
        private double mLancesLivresPercentual;
        private int mRebotes;
        private int mAssistencias;
        private int mBloqueios;
        private int mRoubos;
        private int mInversoesPosse;
        private int mPontos;

        public string Data
        {
            get { return Convert.ToDateTime(mData).ToString("dd/MM/yyyy"); }
            set { mData = value; }
        }

        public string Time
        {
            get { return mTime; }
            set { mTime = value; }
        }

        public string Adversario
        {
            get { return mAdversario; }
            set { mAdversario = value; }
        }

        public string Resultado
        {
            get { return mResultado; }
            set { mResultado = value; }
        }

        public string Minutos
        {
            get { return mMinutos.Substring(0, mMinutos.IndexOf(":")); }
            set { mMinutos = value; }
        }

        public int FieldGoalsFeitos
        {
            get { return (mFieldGoalsFeitos - mCestas3Feitas); }
            set { mFieldGoalsFeitos = value; }
        }

        public int FieldGoalsTentados
        {
            get { return (mFieldGoalsTentados - mCestas3Tentadas); }
            set { mFieldGoalsTentados = value; }
        }

        public double FieldGoalPercentual
        {
            get { return Math.Round(((double)(mFieldGoalsFeitos * 100) / mFieldGoalsTentados), 1); }
        }

        public int Cestas3Feitas
        {
            get { return mCestas3Feitas; }
            set { mCestas3Feitas = value; }
        }

        public int Cestas3Tentadas
        {
            get { return mCestas3Tentadas; }
            set { mCestas3Tentadas = value; }
        }

        public double Cestas3Percentual
        {
            get { return mCestas3Percentual * 100; }
            set { mCestas3Percentual = value; }
        }

        public int LancesLivresFeitos
        {
            get { return mLancesLivresFeitos; }
            set { mLancesLivresFeitos = value; }
        }

        public int LancesLivresTentados
        {
            get { return mLancesLivresTentados; }
            set { mLancesLivresTentados = value; }
        }

        public double LancesLivresPercentual
        {
            get { return mLancesLivresPercentual * 100; }
            set { mLancesLivresPercentual = value; }
        }

        public int Rebotes
        {
            get { return mRebotes; }
            set { mRebotes = value; }
        }

        public int Assistencias
        {
            get { return mAssistencias; }
            set { mAssistencias = value; }
        }

        public int Bloqueios
        {
            get { return mBloqueios; }
            set { mBloqueios = value; }
        }

        public int Roubos
        {
            get { return mRoubos; }
            set { mRoubos = value; }
        }

        public int InversoesPosse
        {
            get { return mInversoesPosse; }
            set { mInversoesPosse = value; }
        }

        public int Pontos
        {
            get { return mPontos; }
            set { mPontos = value; }
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

                if (mPontos >= 10)
                {
                    aux++;
                }

                if (mAssistencias >= 10)
                {
                    aux++;
                }

                if (mRebotes >= 10)
                {
                    aux++;
                }

                if (mBloqueios >= 10)
                {
                    aux++;
                }

                if (mRoubos >= 10)
                {
                    aux++;
                }

                return aux;
            }
        }

        public int PontosAssistenciasRebotes
        {
            get { return mPontos + mAssistencias + mRebotes; }
        }

        public int PontosAssistencias
        {
            get { return mPontos + mAssistencias; }
        }

        public int PontosRebotes
        {
            get { return mPontos + mRebotes; }
        }

        public int AssistenciasRebotes
        {
            get { return mAssistencias + mRebotes; }
        }

        public int RoubosBloqueios
        {
            get { return mRoubos + mBloqueios; }
        }

        public static Partida DictionaryDePara(Dictionary<string, string> dic)
        {
            var partida = new Partida();

            foreach (var key in dic.Keys)
            {
                PropertyInfo property = partida.GetType().GetProperty(key);
                property.SetValue(partida, Convert.ChangeType(dic[key].Replace(".", ","), property.PropertyType), null);
            }

            return partida;
        }
    }
}