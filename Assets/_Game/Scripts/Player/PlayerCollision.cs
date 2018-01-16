using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public partial class Player
{
    public delegate void PlayerDeathHandler();
    public event PlayerDeathHandler OnPlayerDeath;

    public delegate void EnemyHitHandler(GameObject go);
    public event EnemyHitHandler OnEnemyHit;

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
        if (hit.tag.Contains("Target"))
        {
            AudioManager.Instance.PlaySound("TargetGet");
            //ToDo - play animation get
            //ToDo - capture object
        }
        else if (hit.tag.Contains("Obstacle"))
        {
            TakeDamage();
            StartCoroutine(DisableCollisionForXSeconds(invincibilityTime));
            animator.SetTrigger("DamageTaken");
        }
        else if (hit.tag.Contains("Relax"))
        {
            AudioManager.Instance.PlaySound("BonusGet");
        }
    }

    private IEnumerator DisableCollisionForXSeconds(int i)
    {
        var coll = this.GetComponent<Collider2D>();
        coll.enabled = false;
        yield return new WaitForSeconds(i);
        coll.enabled = true;
    }

    [Button("Take Damage")]
    private void TakeDamage()
    {
        if (isPlayerDead)
            return;

        heartPoints--;

        AudioManager.Instance.PlaySound("PlayerDamage");

        if (heartPoints > 0)
            return;

        isPlayerDead = true;
        OnPlayerDeath?.Invoke();
    }
}
