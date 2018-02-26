using System;

namespace Ibit.Calibration
{
    public class CalibrationLogger : Logger<CalibrationLogger>
    {
        protected override void Awake() => sb.AppendLine("time;result;exercise;value");

        public void Write(CalibrationExerciseResult cr, CalibrationExercise ce, float value) => sb.AppendLine($"{DateTime.Now:s};{cr};{ce};{value}");

        protected override void Flush()
        {
            if (sb.Length < 0)
                return;

            var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".csv";
            FileReader.WriteAllText(path, sb.ToString());
        }
    }
}