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

            path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/Calibration-History.csv";

            if (!File.Exists(path))
                sb.AppendLine("dateTime;result;exercise;value");
        }

        public void Write(CalibrationExerciseResult result, CalibrationExercise exercise, float value)
        {
            if (exercise == CalibrationExercise.ExpiratoryPeak || exercise == CalibrationExercise.InspiratoryPeak)
            {
                sb.AppendLine($"{DateTime.Now:s};{result};{exercise};{FlowMath.ToLitresPerMinute(value)};");
            }
            else
            {
                sb.AppendLine($"{DateTime.Now:s};{result};{exercise};{value / 1000f};");
            }
        }

        public void Save()
        {
            if (sb.Length < 0)
                return;
            
            if (!File.Exists(path))
                FileManager.WriteAllText(path, sb.ToString());
            else
                FileManager.AppendAllText(path, sb.ToString());
        }
    }
}