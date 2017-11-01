// ToDo - Load from CSV

public class PlataformConstants
{
    public TargetHeightMultiplier TargetHeightMultiplier = new TargetHeightMultiplier();
    public ObstacleSizeMultiplier ObstacleSizeMultiplier = new ObstacleSizeMultiplier();
    public SpawnQuantity SpawnQuantity = new SpawnQuantity();
    public TimeLimits TimeLimits = new TimeLimits();
}

public class TargetHeightMultiplier
{
    public readonly float Min = 0.33f;
    public readonly float Med = 0.66f;
    public readonly float Max = 1f;
}

public class ObstacleSizeMultiplier
{
    public readonly float Min = 0.33f;
    public readonly float Med = 0.66f;
    public readonly float Max = 1f;
}

public class SpawnQuantity
{
    public readonly int Stage1 = 7;
    public readonly int Stage2 = 8;
    public readonly int Stage3 = 10;

    public readonly int Stage4 = 11;
    public readonly int Stage5 = 12;
    public readonly int Stage6 = 15;

    public readonly int Stage7 = 15;
    public readonly int Stage8 = 17;
    public readonly int Stage9 = 20;
}

public class TimeLimits
{
    public readonly int World1 = 60;
    public readonly int World2 = 90;
    public readonly int World3 = 120;
}
