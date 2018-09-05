﻿using System;
using System.IO;
using System.Text;
using Ibit.Core.Data;
using Ibit.Core.Game;
using Ibit.Core.Util;

namespace Ibit.Calibration
{
    public class CalibrationLogger
    {
        private StringBuilder _sb;
        private string _pathToSave;

        public CalibrationLogger()
        {
            _sb = new StringBuilder();

            _pathToSave = @"savedata/pacients/" + Pacient.Loaded.Id + @"/Calibration-History.csv";

            if (!File.Exists(_pathToSave))
                _sb.AppendLine("dateTime;result;exercise;value");
        }

        public void Write(CalibrationExerciseResult result, CalibrationExercise exercise, float value)
        {
            if (exercise == CalibrationExercise.ExpiratoryPeak || exercise == CalibrationExercise.InspiratoryPeak)
            {
                _sb.AppendLine($"{DateTime.Now:s};{result};{exercise};{FlowMath.ToLitresPerMinute(value)};");
            }
            else if (exercise == CalibrationExercise.RespiratoryFrequency)
            {
                _sb.AppendLine($"{DateTime.Now:s};{result};{exercise};{value * 60f};");
            }
            else
            {
                _sb.AppendLine($"{DateTime.Now:s};{result};{exercise};{value / 1000f};");
            }
        }

        public void Save()
        {
            if (_sb.Length < 0)
                return;

            if (!File.Exists(_pathToSave))
                FileManager.WriteAllText(_pathToSave, _sb.ToString());
            else
                FileManager.AppendAllText(_pathToSave, _sb.ToString());
        }
    }
}