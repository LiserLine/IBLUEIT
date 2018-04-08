using System;

namespace Ibit.Core.Data
{
    [Serializable]
    public class Pacient
    {
        public static Pacient Loaded;

        public int Id;
        public string Name;
        public DateTime Birthday;
        public Capacities Capacities;
        public string Observations;
        public ConditionType Condition;
        public int UnlockedLevels;
        public float AccumulatedScore;
        public int PlaySessionsDone;
        public bool CalibrationDone;
        public bool HowToPlayDone;
        public float Weight;
        public float Height;
        public float PitacoThreshold;
        public string Ethnicity;

#if UNITY_EDITOR
        static Pacient()
        {
            if (Loaded == null)
                Loaded = new Pacient
                {
                    Id = -1,
                    CalibrationDone = true,
                    HowToPlayDone = true,
                    Condition = ConditionType.Normal,
                    Name = "NetRunner",
                    PlaySessionsDone = 0,
                    UnlockedLevels = 15,
                    AccumulatedScore = 0,
                    Capacities = new Capacities
                    {
                        RespCycleDuration = 3000,
                        ExpPeakFlow = 600,
                        InsPeakFlow = -200,
                        ExpFlowDuration = 6000,
                        InsFlowDuration = 6000
                    }
                };
        }
#endif

        public bool IsCalibrationDone()
        {
            return this.Capacities.RawInsPeakFlow < 0
                   && this.Capacities.RawInsFlowDuration > 0
                   && this.Capacities.RawExpPeakFlow > 0
                   && this.Capacities.RawExpFlowDuration > 0
                   && this.Capacities.RawRespCycleDuration > 0;
        }
    }

    public enum ConditionType
    {
        Restrictive = 1,
        Normal = 2,
        Obstructive = 3
    }
}