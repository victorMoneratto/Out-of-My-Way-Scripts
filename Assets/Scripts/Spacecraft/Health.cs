using System;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace OMWGame {
    public class Health : MonoBehaviour {
        #region Points
        [SerializeField] private IntReference maxPoints;

        [HideInEditorMode]
        [ShowInInspector] private int points = 0;
        
        public int Points => points;
        public bool IsAlive => points > 0;
        #endregion


        #region Can receive
        [ReadOnly, HideInEditorMode]
        [ShowInInspector] public bool CanReceiveDamage { get; set; } = true;

        [ReadOnly, HideInEditorMode]
        [ShowInInspector] public bool CanReceiveHeal { get; set; } = true;
        #endregion


        #region Events
        
        [Serializable]
        class HealthEvent : UnityEvent<GameObject> {} 
        
        [Header("Heal & Damage")]
        [SerializeField] private HealthEvent healedEvent;
        [SerializeField] private HealthEvent damagedEvent;

        [Header("Revive & Death")]
        [SerializeField] private HealthEvent reviveEvent;
        [SerializeField] private HealthEvent deathEvent;
        #endregion

        
        public int ReceiveDamage(int amount, IDamager damager) {
            var match = FindObjectOfType<MatchMode>();
            Assert.IsNotNull(match);

            int pointsBefore = points;
            bool rulesAllow = match.CanDamage(damager, gameObject);
            if (amount > 0 && rulesAllow && CanReceiveDamage) {
                bool wasAlive = IsAlive;
                points = Mathf.Clamp(points - amount, 0, maxPoints);
                damagedEvent.Invoke(gameObject);

                if (!IsAlive && wasAlive) {
                    deathEvent.Invoke(gameObject);
                }
            }

            return pointsBefore - points;
        }

        public int ReceiveHeal(int amount) {
            int pointsBefore = points;
            if (amount > 0 && CanReceiveHeal) {
                bool wasAlive = IsAlive;
                points = Mathf.Clamp(points + amount, 0, maxPoints);
                healedEvent.Invoke(gameObject);

                if (IsAlive && !wasAlive) {
                    reviveEvent.Invoke(gameObject);
                }
            }

            return pointsBefore - points;
        }

        public void Revive() {
            if (!IsAlive) {
                ReceiveHeal(maxPoints);
            }
        }
        
        [ContextMenu("Kill")]
        public void Die() {
            if (IsAlive) {
                ReceiveDamage(points, null);
            }
        }

        private void OnEnable() {
            // TODO @OMW assumes that players can be damaged on spawn/revived
            // it will probably not be true later
            CanReceiveDamage = true;
            CanReceiveHeal = true;
        }
    }
}