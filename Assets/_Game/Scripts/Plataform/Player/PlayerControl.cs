using Ibit.Core.Data;
using Ibit.Core.Game;
using Ibit.Core.Util;
using Ibit.Plataform.Camera;
using UnityEngine;

namespace Ibit.Plataform
{
    public partial class Player
    {
        private void PositionOnSerial(string msg)
        {
            if (msg.Length < 1)
                return;

            var sensorValue = Parsers.Float(msg);

            sensorValue = sensorValue < -GameManager.PitacoFlowThreshold || sensorValue > GameManager.PitacoFlowThreshold ? sensorValue : 0f;

            var peak = sensorValue > 0 ? Pacient.Loaded.Capacities.ExpPeakFlow * 0.5f : -Pacient.Loaded.Capacities.InsPeakFlow;

            var nextPosition = sensorValue * CameraLimits.Boundary / peak;

            nextPosition = Mathf.Clamp(nextPosition, -CameraLimits.Boundary, CameraLimits.Boundary);

            var from = this.transform.position;
            var to = new Vector3(this.transform.position.x, -nextPosition, this.transform.position.z);

            this.transform.position = Vector3.Lerp(from, to, Time.deltaTime * 9f);
        }
    }
 }