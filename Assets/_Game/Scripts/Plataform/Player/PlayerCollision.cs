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

        public Action OnPlayerDeath;
        public Action<GameObject> OnObjectHit;

        #endregion Events

        [SerializeField] [BoxGroup("Animation Control")] private Animator animator;
        [SerializeField] [BoxGroup("Properties")] private int invincibilityTime = 2;

        private bool isPlayerDead;

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
                StartCoroutine(DisableCollisionForXSeconds(invincibilityTime));
                SoundManager.Instance.PlaySound("PlayerDamage");
            }
            else if (hit.CompareTag("RelaxObject"))
            {
                SoundManager.Instance.PlaySound("BonusTargetGet", true);
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
}