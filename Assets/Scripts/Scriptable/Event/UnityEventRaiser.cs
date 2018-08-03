using System;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame.Scriptable {
    public class UnityEventRaiser : MonoBehaviour {
        
        [Serializable]
        class InvokeEvent : UnityEvent<GameObject> { }
        
        [SerializeField] private InvokeEvent awake;
        
        [SerializeField] private InvokeEvent start;
        
        [SerializeField] private InvokeEvent enable;
        
        [SerializeField] private InvokeEvent disable;
        
        [SerializeField] private InvokeEvent destroy;

        private void Awake() {
            awake.Invoke(gameObject);
        }

        private void Start() {
            start.Invoke(gameObject);
        }

        private void OnEnable() {
            enable.Invoke(gameObject);
        }

        private void OnDisable() {
            disable.Invoke(gameObject);
        }

        private void OnDestroy() {
            destroy.Invoke(gameObject);
        }
    }
}