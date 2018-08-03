using Boo.Lang;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class StageSafeZone : MonoBehaviour {
        [ShowInInspector, HideInEditorMode]
        private List<GameObject> playersInside;

        [Required]
        [SerializeField] private GameObjectRunSet playersAlive;

        [ShowInInspector, HideInEditorMode]
        private bool waiting;

        [SerializeField] private UnityEvent stageReadyEvent;

        public void BeginWaitingForPlayers() {
            waiting = true;
            if (playersInside == null) {
                playersInside = new List<GameObject>();
            } else {
                playersInside.Clear();
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!waiting) return;
            
            if (playersAlive.Items.Contains(other.gameObject)) {
                playersInside.AddUnique(other.gameObject);
            }

            if (playersAlive.Count == playersInside.Count) {
                waiting = false;
                playersInside.Clear();

                stageReadyEvent.Invoke();
            }
        }

        private void OnCollisionExit(Collision other) {
            if (!waiting) return;
            
            if (playersAlive.Items.Contains(other.gameObject)) {
                playersInside.Remove(other.gameObject);
            }
        }
    }
}