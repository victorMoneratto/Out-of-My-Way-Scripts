using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame.Scriptable {
    public class IntEventListener : MonoBehaviour {
        [Serializable]
        public class ResponseType : UnityEvent<int> {}
        
        [HideLabel, Required]
        public IntEvent Event;
        public ResponseType Response;

        private void OnEnable() {
            Event.RegisterListener(this);
        }

        private void OnDisable() {
            Event.RemoveListener(this);
        }

        public void OnEventRaised(int value) {
            Response.Invoke(value);
        }
    }
}