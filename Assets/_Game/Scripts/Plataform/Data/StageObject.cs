using System;

namespace Ibit.Plataform.Data
{
    [Serializable]
    public class StageObject
    {
        public int Id;
        public StageObjectType Type;
        public float DifficultyFactor;
        public float PositionYFactor;
        public float PositionXSpacing;
    }
}
