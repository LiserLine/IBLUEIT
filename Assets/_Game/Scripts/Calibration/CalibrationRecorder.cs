using System;
using System.Text;

public class CalibrationRecorder : Recorder<CalibrationRecorder>
{
    private StringBuilder sb;

    private void Awake() => sb = new StringBuilder();

    private void Start()
    {
        StartRecord();
        CalibrationManager.Instance.OnCalibrationEnd += StopRecord;
    }

    public void Write(CalibrationExercise ce, float value) => sb.AppendLine($"[{DateTime.Now}] {ce}: {value}");

    protected override void StopRecord()
    {
        base.StopRecord();
        Flush();
    }

    private void Flush()
    {
        if (sb.Length < 0)
            return;

        var path = @"savedata/pacients/" + Pacient.Loaded.Id + @"/" + $"{recordStart:yyyyMMdd-HHmmss}_" + FileName + ".txt";
        Utils.WriteAllText(path, sb.ToString());
    }
}