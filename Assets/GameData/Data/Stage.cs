using System;

public class Stage
{
    public delegate void StageStartHandler();
    public event StageStartHandler OnStageStart;

    public delegate void StageEndHandler();
    public event StageEndHandler OnStageEnd;

    public int Id { get; set; }
    public bool IsRunning { get; set; }

    public virtual void Start()
    {
        if (IsRunning)
            throw new Exception("Stage is already running.");

        OnStageStart?.Invoke();
        IsRunning = true;
    }

    public virtual void Stop()
    {
        if (!IsRunning)
            return;

        OnStageEnd?.Invoke();
        IsRunning = false;
    }
}

public class PlataformStage : Stage
{
    /// <summary>
    /// Stage time limit
    /// </summary>
    public int TimeLimit { get; private set; }

    /// <summary>
    /// Objects to spawn during the game
    /// </summary>
    public int SpawnQuantitity { get; private set; }

    /// <summary>
    /// Spawn delay for Spawner (SpawnQuantitity / TimeLimit)
    /// </summary>
    public int SpawnDelay
    {
        get
        {
            if (!IsRunning)
                throw new Exception("Plataform is not running. Start a stage to use SpawnDelay.");

            return SpawnQuantitity / TimeLimit;
        }
    }

    /// <summary>
    /// Elements used on the game.
    /// </summary>
    public PlataformElements Elements { get; private set; }

    /// <summary>
    /// Multiplier used to define max and min sizes of obstacles.
    /// </summary>
    public float ObstacleSizeMultiplier { get; private set; }

    /// <summary>
    /// Multiplier used to define max and min height of targets.
    /// </summary>
    public float TargetHeightMultiplier { get; private set; }

    /// <summary>
    /// Initializes the Stage variables and start the stage.
    /// </summary>
    public override void Start()
    {
        if (Id >= 1 && Id <= 3)
        {
            Elements = PlataformElements.Targets;
            TargetHeightMultiplier = Id == 3 ? GameConstants.Plataform.TargetHeightMultiplier.Med : GameConstants.Plataform.TargetHeightMultiplier.Min;
            SpawnQuantitity = Id == 1 ? GameConstants.Plataform.SpawnQuantity.Stage1 : (Id == 2 ? GameConstants.Plataform.SpawnQuantity.Stage2 : GameConstants.Plataform.SpawnQuantity.Stage3);
            TimeLimit = GameConstants.Plataform.TimeLimits.World1;
        }
        else if (Id >= 4 && Id <= 6)
        {
            Elements = PlataformElements.TargetAndObstacles;
            TargetHeightMultiplier = Id == 4 ? GameConstants.Plataform.TargetHeightMultiplier.Med : GameConstants.Plataform.TargetHeightMultiplier.Max;
            ObstacleSizeMultiplier = Id == 6 ? GameConstants.Plataform.ObstacleSizeMultiplier.Med : GameConstants.Plataform.ObstacleSizeMultiplier.Min;
            SpawnQuantitity = Id == 4 ? GameConstants.Plataform.SpawnQuantity.Stage4 : (Id == 5 ? GameConstants.Plataform.SpawnQuantity.Stage5 : GameConstants.Plataform.SpawnQuantity.Stage6);
            TimeLimit = GameConstants.Plataform.TimeLimits.World2;
        }
        else if (Id >= 7 && Id <= 9)
        {
            Elements = PlataformElements.Obstacles;
            ObstacleSizeMultiplier = Id == 7 ? GameConstants.Plataform.ObstacleSizeMultiplier.Med : GameConstants.Plataform.ObstacleSizeMultiplier.Max;
            SpawnQuantitity = Id == 7 ? GameConstants.Plataform.SpawnQuantity.Stage7 : (Id == 8 ? GameConstants.Plataform.SpawnQuantity.Stage8 : GameConstants.Plataform.SpawnQuantity.Stage9);
            TimeLimit = GameConstants.Plataform.TimeLimits.World3;
        }

        base.Start();
    }
}
