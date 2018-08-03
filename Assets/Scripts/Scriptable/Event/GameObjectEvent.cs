using System.Collections.Generic;
using UnityEngine;

namespace OMWGame.Scriptable {
    [CreateAssetMenu(fileName = "On Event", menuName = "Event/GameObject")]
    public class GameObjectEvent : ScriptableObject {
        private readonly List<GameObjectEventListener> listeners = new List<GameObjectEventListener>();

        public void Raise(GameObject go) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised(go);
            }
        }

        public void RegisterListener(GameObjectEventListener listener) {
            if (!listeners.Contains(listener)) {
                listeners.Add(listener);
            }
        }

        public void RemoveListener(GameObjectEventListener listener) {
            listeners.Remove(listener);
        }
    }
}