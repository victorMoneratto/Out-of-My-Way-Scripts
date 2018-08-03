using System.Collections.Generic;
using UnityEngine;

namespace OMWGame.Scriptable {
    [CreateAssetMenu(fileName = "On Event", menuName = "Event/No Params")]
    public class GameEvent : ScriptableObject {
        protected readonly List<GameEventListener> Listeners = new List<GameEventListener>();

        public void Raise() {
            for (int i = Listeners.Count - 1; i >= 0; i--) {
                Listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener) {
            if (!Listeners.Contains(listener)) {
                Listeners.Add(listener);
            }
        }

        public void RemoveListener(GameEventListener listener) {
            Listeners.Remove(listener);
        }
    }
}