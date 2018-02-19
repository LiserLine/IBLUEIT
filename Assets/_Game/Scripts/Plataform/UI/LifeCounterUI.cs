using UnityEngine;
using UnityEngine.UI;

public class LifeCounterUI : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;

    private float startHP;
    private Player plr;

    private void Awake()
    {
        plr = FindObjectOfType<Player>();
        startHP = plr.HeartPoins;
        plr.OnEnemyHit += UpdateHeartPoints;
    }

    private void UpdateHeartPoints(GameObject go) => fillImage.fillAmount = plr.HeartPoins / startHP;
}