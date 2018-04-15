using Ibit.Plataform.Camera;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Spawn;
using UnityEngine;

namespace Ibit.Plataform
{
    public partial class Target : MonoBehaviour
    {
        public StageObject Properties;

        public float Score { get; private set; }        

        public void Build(StageObject properties)
        {
            this.Properties = properties;
            CalculateHeight();
            CalculateScore();
            FindObjectOfType<Spawner>().OnUpdatedPerformanceTarget += OnUpdatedPerformance;
        }

        private void OnUpdatedPerformance(float insAcc, float expAcc)
        {
            CalculateHeight(this.Properties.PositionYFactor > 0 ? insAcc : expAcc);
        }

        private void CalculateScore()
        {
            Score = Mathf.Abs(this.transform.position.y) * (1f + this.Properties.DifficultyFactor) * 1000f;
        }

        private void CalculateHeight(float performanceAccumulator = 0)
        {
            var tmpPos = this.transform.position;

            tmpPos.y = (1f + performanceAccumulator) * CameraLimits.Boundary * this.Properties.DifficultyFactor;

            tmpPos.y = this.Properties.PositionYFactor > 0 ?
                Mathf.Clamp(tmpPos.y, 0.2f * CameraLimits.Boundary, CameraLimits.Boundary) :
                Mathf.Clamp(-tmpPos.y, -CameraLimits.Boundary, 0.2f * -CameraLimits.Boundary);

            this.transform.position = tmpPos;
        }

        private void OnDestroy()
        {
            FindObjectOfType<Spawner>().OnUpdatedPerformanceTarget -= OnUpdatedPerformance;
        }
    }
}