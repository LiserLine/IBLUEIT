using Ibit.Plataform.Data;
using UnityEngine;

namespace Ibit.Plataform
{
    public partial class Obstacle : MonoBehaviour
    {
        [SerializeField] private int heartPoint = 1;

        public StageObject Properties;
        public int HeartPoint => heartPoint;
    }
}