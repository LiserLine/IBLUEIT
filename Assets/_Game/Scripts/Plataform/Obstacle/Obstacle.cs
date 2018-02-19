using UnityEngine;

public partial class Obstacle : MonoBehaviour
{
    public int HeartPoint => heartPoint;

    [SerializeField]
    private int heartPoint = 1;
}