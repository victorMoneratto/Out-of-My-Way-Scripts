using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame.Scriptable {
    public class GameObjectEventListener : MonoBehaviour {
        [Serializable]
        public class ResponseType : UnityEvent<GameObject> {}
        
        [HideLabel, Required]
        public GameObjectEvent Event;
        public ResponseType Response;

        private void OnEnable() {
            Event.RegisterListener(this);
        }

        private void OnDisable() {
            Event.RemoveListener(this);
        }

        public void OnEventRaised(GameObject go) {
            Response.Invoke(go);
        }
    }
}