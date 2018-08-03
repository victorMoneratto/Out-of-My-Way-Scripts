using System.Collections.Generic;
using UnityEngine;

namespace OMWGame.Scriptable {
    [CreateAssetMenu(fileName = "On Event", menuName = "Event/Integer")]
    public class IntEvent : ScriptableObject {
        private readonly List<IntEventListener> listeners = new List<IntEventListener>();

        public void Raise(int value) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised(value);
            }
        }

        public void RegisterListener(IntEventListener listener) {
            if (!listeners.Contains(listener)) {
                listeners.Add(listener);
            }
        }

        public void RemoveListener(IntEventListener listener) {
            listeners.Remove(listener);
        }
    }
}