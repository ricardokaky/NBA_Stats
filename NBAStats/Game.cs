using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBAStats
{
    public class Game
    {
        private string mDate;
        private string mOpponent;
        private string mResult;
        private string mMinutes;
        private int mFieldGoalsMade;
        private int mFieldGoalsAttempted;
        private int mTreePointFieldGoalsMade;
        private int mTreePointFieldGoalsAttempted;
        private double mTreePointFieldGoalPercentage;
        private int mFreeThrowsMade;
        private int mFreeThrowsAttempted;
        private double mFreeThrowPercentage;
        private int mRebounds;
        private int mAssists;
        private int mBlocks;
        private int mSteals;
        private int mTurnovers;
        private int mPoints;

        public string Date
        {
            get { return Convert.ToDateTime(mDate).ToString("dd/MM/yyyy"); }
            set { mDate = value; }
        }
        public string Opponent
        {
            get { return mOpponent; }
            set { mOpponent = value; }
        }
        public string Result
        {
            get { return mResult; }
            set { mResult = value; }
        }

        public string Minutes
        {
            get { return mMinutes.Substring(0, mMinutes.IndexOf(":")); }
            set { mMinutes = value; }
        }
        public int FieldGoalsMade
        {
            get { return (mFieldGoalsMade - mTreePointFieldGoalsMade); }
            set { mFieldGoalsMade = value; }
        }

        public int FieldGoalsAttempted
        {
            get { return (mFieldGoalsAttempted - mTreePointFieldGoalsAttempted); }
            set { mFieldGoalsAttempted = value; }
        }

        public double FieldGoalPercentage
        {
            get { return Math.Round(((double)(mFieldGoalsMade * 100) / mFieldGoalsAttempted), 1); }
        }

        public int TreePointFieldGoalsMade
        {
            get { return mTreePointFieldGoalsMade; }
            set { mTreePointFieldGoalsMade = value; }
        }

        public int TreePointFieldGoalsAttempted
        {
            get { return mTreePointFieldGoalsAttempted; }
            set { mTreePointFieldGoalsAttempted = value; }
        }

        public double TreePointFieldGoalPercentage
        {
            get { return mTreePointFieldGoalPercentage * 100; }
            set { mTreePointFieldGoalPercentage = value; }
        }

        public int FreeThrowsMade
        {
            get { return mFreeThrowsMade; }
            set { mFreeThrowsMade = value; }
        }

        public int FreeThrowsAttempted
        {
            get { return mFreeThrowsAttempted; }
            set { mFreeThrowsAttempted = value; }
        }

        public double FreeThrowPercentage
        {
            get { return mFreeThrowPercentage * 100; }
            set { mFreeThrowPercentage = value; }
        }

        public int Rebounds
        {
            get { return mRebounds; }
            set { mRebounds = value; }
        }

        public int Assists
        {
            get { return mAssists; }
            set { mAssists = value; }
        }

        public int Blocks
        {
            get { return mBlocks; }
            set { mBlocks = value; }
        }

        public int Steals
        {
            get { return mSteals; }
            set { mSteals = value; }
        }

        public int Turnovers
        {
            get { return mTurnovers; }
            set { mTurnovers = value; }
        }

        public int Points
        {
            get { return mPoints; }
            set { mPoints = value; }
        }
        
        public int DoubleAndTripleDouble
        {
            get
            {
                int aux = 0;

                if (mPoints >= 10)
                {
                    aux++;
                }

                if (mAssists >= 10)
                {
                    aux++;
                }

                if (mRebounds >= 10)
                {
                    aux++;
                }

                if (mBlocks >= 10)
                {
                    aux++;
                }

                if (mSteals >= 10)
                {
                    aux++;
                }

                return aux;
            }
        }

        public int PointsAssistsRebounds
        {
            get { return mPoints + mAssists + mRebounds; }
        }

        public int PointsAssists
        {
            get { return mPoints + mAssists; }
        }

        public int PointsRebounds
        {
            get { return mPoints + mRebounds; }
        }

        public int AssistsRebounds
        {
            get { return mAssists + mRebounds; }
        }

        public int StealsBlocks
        {
            get { return mSteals + mBlocks; }
        }

        public void FromDictionary(Dictionary<string, string> dic)
        {
            foreach (var key in dic.Keys)
            {
                PropertyInfo property = GetType().GetProperty(key);
                property.SetValue(this, Convert.ChangeType(dic[key].Replace(".", ","), property.PropertyType), null);
            }
        }
    }
}