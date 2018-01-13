using UnityEngine;
using UnityEngine.UI;

public class RelaxCoinUI : MonoBehaviour
{
    [SerializeField]
    private Image fillSprite;

    private bool filled;

    private float coinsHit;

    private void Awake()
    {
        Player.Instance.OnEnemyHit += UpdateSprite;
    }

    private void UpdateSprite(GameObject go)
    {
        if (filled)
            return;

        if (!go.tag.Equals("RelaxCoin"))
            return;

        coinsHit++;

        fillSprite.fillAmount = coinsHit / Spawner.Instance.RelaxTrigger;

        if (fillSprite.fillAmount < 0.95f)
            return;

        filled = true;
        fillSprite.fillAmount = 1f;
    }
}
