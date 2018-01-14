using UnityEngine;
using UnityEngine.UI;

public class LifeCounter : MonoBehaviour
{
    [SerializeField]
    private Image heartSprite;

    private float startHP;

    private void Awake()
    {
        startHP = Player.Instance.HeartPoins;
        Player.Instance.OnEnemyHit += UpdateHeartPoints;
    }

    private void UpdateHeartPoints(GameObject go)
    {
        heartSprite.fillAmount = Player.Instance.HeartPoins / startHP;
    }
}
