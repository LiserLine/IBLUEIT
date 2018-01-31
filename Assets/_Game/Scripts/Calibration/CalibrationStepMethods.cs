public partial class CalibrationManager
{
    /// <summary>
    /// Method to execute next step of calibration.
    /// Some buttons use this to execute the next step.
    /// </summary>
    public void NextStep() => runNextStep = true;

    /// <summary>
    /// Sets a step to be executed on next step iteration.
    /// </summary>
    /// <param name="step">Step number</param>
    /// <param name="jumpToStep">Flag to execute the next step automatically</param>
    private void SetStep(int step, bool jumpToStep = false)
    {
        currentStep = step;
        runNextStep = jumpToStep;
    }

    /// <summary>
    /// Sets the next step to be executed on next step iteration.
    /// </summary>
    /// <param name="jumpToStep">Flag to execute the next step automatically</param>
    private void SetNextStep(bool jumpToStep = false)
    {
        currentStep++;
        runNextStep = jumpToStep;
    }
}