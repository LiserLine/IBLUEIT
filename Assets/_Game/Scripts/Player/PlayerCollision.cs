using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public partial class Player
{
    public delegate void PlayerDeathHandler();
    public static event PlayerDeathHandler OnPlayerDeath;

    public delegate void EnemyHitHandler(GameObject go);
    public static event EnemyHitHandler OnEnemyHit;

    [SerializeField]
    private int invincibilityTime = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HitResult(collision.gameObject);
        OnEnemyHit?.Invoke(collision.gameObject);
    }

    private void HitResult(GameObject hit)
    {
        if (hit.tag.Contains("Target"))
        {
            Destroy(hit);
            //ToDo - play animation get
            //ToDo - capture object
        }
        else if (hit.tag.Contains("Obstacle"))
        {
            TakeDamage();
            StartCoroutine(DisableCollisionForXSeconds(invincibilityTime));
            //todo - play ghost animation
            //todo - disable collision for 3 seconds
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
        heartPoints--;

        if (heartPoints <= 0)
            OnPlayerDeath?.Invoke();
    }
}
