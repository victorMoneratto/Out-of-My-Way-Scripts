using Rewired;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    public class StartGameView : MonoBehaviour {
        [SerializeField] private Transform progressBar;
        
        [SerializeField] private float timeToHold = 1;
        
        [SerializeField] private UnityEvent startPlayEvent;

        private void Awake() {
            var barScale = progressBar.localScale;
            barScale.x = 0;
            progressBar.localScale = barScale;
        }

        private void Update() {
            if (!ReInput.isReady) { return; }
            
            // progress bar
            float timeHeldButtonToPlay = ReInput.players.SystemPlayer.GetButtonTimePressed("Start");
            // visual percentage has a little bump to make 100% visible
            float timeHeldVisualPercentage = (timeHeldButtonToPlay/timeToHold) * 1.05f;
            var barScale = progressBar.localScale;
            barScale.x = Mathf.Clamp01(timeHeldVisualPercentage); 
            progressBar.localScale = barScale;
            
            // play
            bool play = ReInput.players.SystemPlayer.GetButtonTimedPressDown("Start", timeToHold);
            if (play) {
                startPlayEvent.Invoke();
            }
        }
    }
}