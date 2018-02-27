using Ibit.Plataform.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.Plataform.UI
{
    public class RelaxCoinUI : MonoBehaviour
    {
        [SerializeField]
        private Image fillSprite;

        private bool filled;
        private float coinsHit;

        private void Awake() => FindObjectOfType<Ibit.Plataform.Player>().OnEnemyHit += UpdateSprite;

        private void UpdateSprite(GameObject go)
        {
            if (filled)
                return;

            if (!go.CompareTag("RelaxCoin"))
                return;

            coinsHit++;

            fillSprite.fillAmount = coinsHit / Stage.Loaded.RelaxTimeThreshold;

            if (fillSprite.fillAmount < 0.95f)
                return;

            filled = true;
            fillSprite.fillAmount = 1f;
        }
    }
}