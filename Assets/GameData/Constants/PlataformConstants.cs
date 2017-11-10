// ToDo - Load from CSV

public class PlataformConstants
{
    public TargetHeightMultiplier TargetHeightMultiplier = new TargetHeightMultiplier();
    public ObstacleSizeMultiplier ObstacleSizeMultiplier = new ObstacleSizeMultiplier();
    public SpawnQuantity SpawnQuantity = new SpawnQuantity();
    public TimeLimits TimeLimits = new TimeLimits();

    public float NextStageOpenProportion = 0.7f;
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
    public readonly int Stage1_1 = 5;
    public readonly int Stage1_2 = 6;
    public readonly int Stage1_3 = 6;

    public readonly int Stage2_1 = 8;
    public readonly int Stage2_2 = 9;
    public readonly int Stage2_3 = 10;

    public readonly int Stage3_1 = 13;
    public readonly int Stage3_2 = 14;
    public readonly int Stage3_3 = 15;
}

public class TimeLimits
{
    public readonly int World1 = 60;
    public readonly int World2 = 90;
    public readonly int World3 = 120;
}
