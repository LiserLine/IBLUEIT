using Ibit.Core.Data;
using Ibit.Plataform.Data;
using Ibit.Plataform.Manager.Spawn;
using UnityEngine;

namespace Ibit.Plataform
{
    public partial class Obstacle : MonoBehaviour
    {
        [SerializeField] private int heartPoint = 1;

        public StageObject Properties;
        public int HeartPoint => heartPoint;

        public float Score { get; private set; }

        public void Build(StageObject properties)
        {
            this.Properties = properties;
            CalculateSize();
            CalculateScore();
            FindObjectOfType<Spawner>().OnUpdatedPerformanceObstacle += OnUpdatedPerformance;
        }

        private void OnUpdatedPerformance(float insAcc, float expAcc)
        {
            CalculateSize(this.Properties.PositionYFactor > 0 ? expAcc : insAcc);
        }

        private void CalculateSize(float performanceFactor = 0)
        {
            var tmpScale = this.transform.localScale;

            tmpScale.x = (this.Properties.PositionYFactor > 0 ? Pacient.Loaded.Capacities.ExpFlowDuration : Pacient.Loaded.Capacities.InsFlowDuration)
                / 1000f * (1f + performanceFactor) * this.Properties.DifficultyFactor;

            tmpScale.x = tmpScale.x < 1f ? 1f : tmpScale.x;

            this.transform.localScale = new Vector3(tmpScale.x, tmpScale.x);

            var spriteOffset = this.transform.localScale.y / 2f;

            this.transform.position = new Vector3(this.transform.position.x, this.Properties.PositionYFactor > 0 ? spriteOffset : -spriteOffset);
        }

        private void CalculateScore()
        {
            Score = this.transform.localScale.x * (1f + this.Properties.DifficultyFactor) * 1000f;
        }

        private void OnDestroy()
        {
            FindObjectOfType<Spawner>().OnUpdatedPerformanceObstacle -= OnUpdatedPerformance;
        }
    }
}