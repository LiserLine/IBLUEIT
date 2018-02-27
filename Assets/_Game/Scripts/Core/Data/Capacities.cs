using Ibit.Core.Game;

namespace Ibit.Core.Data
{
    public class Capacities
    {
        public float RawInsPeakFlow { get; private set; }
        public float RawExpPeakFlow { get; private set; }
        public float RawInsFlowDuration { get; private set; }
        public float RawExpFlowDuration { get; private set; }
        public float RawRespCycleDuration { get; private set; }

        public float InsPeakFlow
        {
            get { return RawInsPeakFlow * GameManager.CapacityMultiplier; }
            set { RawInsPeakFlow = value; }
        }

        public float ExpPeakFlow
        {
            get { return RawExpPeakFlow * GameManager.CapacityMultiplier; }
            set { RawExpPeakFlow = value; }
        }

        /// <summary>
        /// Inspiratory Flow Time (in milliseconds)
        /// </summary>
        public float InsFlowDuration
        {
            get { return RawInsFlowDuration * GameManager.CapacityMultiplier; }
            set { RawInsFlowDuration = value; }
        }

        /// <summary>
        /// Expiratory Flow Time (in milliseconds)
        /// </summary>
        public float ExpFlowDuration
        {
            get { return RawExpFlowDuration * GameManager.CapacityMultiplier; }
            set { RawExpFlowDuration = value; }
        }

        /// <summary>
        /// Respiration Frequency (mean time of one cycle, in milliseconds)
        /// </summary>
        public float RespCycleDuration
        {
            get { return RawRespCycleDuration / GameManager.CapacityMultiplier; }
            set { RawRespCycleDuration = value; }
        }

        public void Reset()
        {
            RawInsPeakFlow = 0f;
            RawExpPeakFlow = 0f;
            RawInsFlowDuration = 0f;
            RawExpFlowDuration = 0f;
            RawRespCycleDuration = 0f;
        }
    }
}