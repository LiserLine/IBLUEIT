﻿namespace Ibit.Calibration
{
    public partial class CalibrationManager
    {
        /// <summary>
        /// Method to execute next step of calibration.
        /// Some buttons use this to execute the next step.
        /// </summary>
        public void NextStep() => _runStep = true;

        /// <summary>
        /// Sets a step to be executed on next step iteration.
        /// </summary>
        /// <param name="step">Step number</param>
        /// <param name="jumpToStep">Flag to execute the next step automatically</param>
        private void SetupStep(int step, bool jumpToStep = false)
        {
            _currentStep = step;
            _runStep = jumpToStep;
        }

        /// <summary>
        /// Sets the next step to be executed on next step iteration.
        /// </summary>
        /// <param name="jumpToStep">Flag to execute the next step automatically</param>
        private void SetupNextStep(bool jumpToStep = false)
        {
            _currentStep++;
            _runStep = jumpToStep;
        }
    }
}