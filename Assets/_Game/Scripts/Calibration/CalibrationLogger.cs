using Ibit.Core.Data;
using Ibit.Core.Util;
using System;
using System.IO;
using System.Text;
using Ibit.Core.Game;

namespace Ibit.Calibration
{
    public class CalibrationLogger
    {
        private StringBuilder sb;
        private string path;

        public CalibrationLogger()
        {
            sb = new StringBuilder();

            path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/"+ $"{GameManager.GameStart:yyyyMMdd-HHmmss}-Calibration.csv";

            if (!File.Exists(path))
                sb.AppendLine("dateTime;result;exercise;value");
        }

        public void Write(CalibrationExerciseResult cr, CalibrationExercise ce, float value)
        {
            if (ce == CalibrationExercise.ExpiratoryPeak || ce == CalibrationExercise.InspiratoryPeak)
            {
                sb.AppendLine($"{DateTime.Now:s};{cr};{ce};{FlowMath.ToLitresPerMinute(value)};");
            }
            else
            {
                sb.AppendLine($"{DateTime.Now:s};{cr};{ce};{value / 1000f};");
            }
        }

        public void Save()
        {
            if (sb.Length < 0)
                return;
            
            if (!File.Exists(path))
                FileReader.WriteAllText(path, sb.ToString());
            else
                FileReader.AppendAllText(path, sb.ToString());
        }
    }
}