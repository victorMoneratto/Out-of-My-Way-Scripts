using System;
using OMWGame.Scriptable;
using Rewired;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    public class HUD : MonoBehaviour {
        
        [SerializeField] private UnityEvent togglePauseEvent;

        public void Update() {
            if (!ReInput.isReady) { return; }

            var system = ReInput.players.SystemPlayer;
            var togglePause = system.GetButtonDown("Start");
            
            togglePause |= system.GetButtonDown("Back");
            
            if (togglePause) {
                togglePauseEvent.Invoke();
            }
        }
    }
}