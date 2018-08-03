using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame.Scriptable {
    public class GameEventListener : MonoBehaviour {
        [HideLabel, Required]
        public GameEvent Event;
        public UnityEvent Response;

        private void OnEnable() {
            Event.RegisterListener(this);
        }

        private void OnDisable() {
            Event.RemoveListener(this);
        }

        public void OnEventRaised() {
            Response.Invoke();
        }
    }
}