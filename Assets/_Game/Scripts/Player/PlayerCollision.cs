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
        if (hit.tag.Equals("WaterTarget") || hit.tag.Equals("AirTarget"))
        {
            //ToDo - play animation get
            //ToDo - capture object
            AudioManager.Instance.PlaySound("TargetGet");
        }
        else if (hit.tag.Equals("WaterObstacle") || hit.tag.Equals("AirObstacle"))
        {
            TakeDamage();
            StartCoroutine(DisableCollisionForXSeconds(invincibilityTime));
            animator.SetTrigger("DamageTaken");
            AudioManager.Instance.PlaySound("PlayerDamage");
        }
        else if (hit.tag.Equals("RelaxCoin"))
        {
            AudioManager.Instance.PlaySound("BonusGet");
        }
        else if (hit.tag.Equals("BonusTarget"))
        {
            AudioManager.Instance.PlaySound("BonusTargetGet");
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

        if (heartPoints > 0)
            return;

        isPlayerDead = true;
        OnPlayerDeath?.Invoke();
    }
}
