using Ibit.Core.Audio;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

namespace Ibit.Plataform
{
    public partial class Player
    {
        #region Events

        public event Action OnPlayerDeath;
        public event Action<GameObject> OnObjectHit;

        #endregion Events

        [SerializeField] [BoxGroup("Animation Control")] private Animator animator;
        [SerializeField] [BoxGroup("Properties")] private int invincibilityTime = 2;

        private bool isPlayerDead;
        private bool isInvulnerable;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            HitResult(collision.gameObject);
            OnObjectHit?.Invoke(collision.gameObject);
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
                SoundManager.Instance.PlaySound("PlayerDamage");
            }
            else if (hit.CompareTag("RelaxObject"))
            {
                SoundManager.Instance.PlaySound("BonusTargetGet", true);
            }
        }

        private IEnumerator DisableCollisionForXSeconds(int i)
        {
            isInvulnerable = true;
            animator.SetBool("DamageTaken", isInvulnerable);

            var component = this.GetComponent<Collider2D>();

            component.enabled = false;
            yield return new WaitForSeconds(i);
            component.enabled = true;

            isInvulnerable = false;
            animator.SetBool("DamageTaken", isInvulnerable);
        }

        [Button("Take Damage")]
        private void TakeDamage()
        {
            if (isPlayerDead || isInvulnerable)
                return;

            StartCoroutine(DisableCollisionForXSeconds(invincibilityTime));

            heartPoints--;

            if (heartPoints > 0)
                return;

            isPlayerDead = true;
            OnPlayerDeath?.Invoke();
        }
    }
}