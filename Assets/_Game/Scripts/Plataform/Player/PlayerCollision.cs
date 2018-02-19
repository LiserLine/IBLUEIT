using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public partial class Player
{
    #region Events

    public delegate void PlayerDeathHandler();

    public event PlayerDeathHandler OnPlayerDeath;

    public delegate void EnemyHitHandler(GameObject go);

    public event EnemyHitHandler OnEnemyHit;

    #endregion Events

    [SerializeField]
    private Animator animator;

    [SerializeField]
    [BoxGroup("Properties")]
    private int invincibilityTime = 2;

    private bool isPlayerDead;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HitResult(collision.gameObject);
        OnEnemyHit?.Invoke(collision.gameObject);
    }

    private void HitResult(GameObject hit)
    {
        if (hit.CompareTag("WaterTarget") || hit.CompareTag("AirTarget"))
        {
            SoundManager.Instance.PlaySound("TargetGet");
        }
        else if (hit.CompareTag("WaterObstacle") || hit.CompareTag("AirObstacle"))
        {
            TakeDamage();
            StartCoroutine(DisableCollisionForXSeconds(invincibilityTime));
            SoundManager.Instance.PlaySound("PlayerDamage");
        }
        else if (hit.CompareTag("RelaxCoin"))
        {
            SoundManager.Instance.PlaySound("BonusGet");
        }
        else if (hit.CompareTag("RelaxTarget"))
        {
            SoundManager.Instance.PlaySound("BonusTargetGet");
        }
    }

    private IEnumerator DisableCollisionForXSeconds(int i)
    {
        var component = this.GetComponent<Collider2D>();
        component.enabled = false;
        yield return new WaitForSeconds(i);
        component.enabled = true;
    }

    [Button("Take Damage")]
    private void TakeDamage()
    {
        if (isPlayerDead)
            return;

        animator.SetTrigger("DamageTaken");

        heartPoints--;

        if (heartPoints > 0)
            return;

        isPlayerDead = true;
        OnPlayerDeath?.Invoke();
    }
}