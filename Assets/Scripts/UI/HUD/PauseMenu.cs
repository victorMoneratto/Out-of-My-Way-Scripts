using System.Collections;
using OMWGame.Scriptable;
using Rewired;
using UnityEngine;

namespace OMWGame {
    public class PauseMenu : MonoBehaviour {

        private float timeScaleBeforePause = 1;

        [SerializeField] private GameObjectRunSet alivePlayers;

        public void TogglePause() {
            bool doPause = !gameObject.activeSelf;
            if (doPause) {
                // time scale pause
                timeScaleBeforePause = Time.timeScale;
                Time.timeScale = 0;
                
                SetUIInputMode(true);
                gameObject.SetActive(true);
            } else {
                // resume time scale
                Time.timeScale = timeScaleBeforePause;
                
                // TODO @OMW spacecrafts receive input the next frame
                // leading them to perform actions with the same
                // button press (e.g. boost when unpausing)
                SetUIInputMode(false);
                gameObject.SetActive(false);
            }
        }

        private void SetUIInputMode(bool ui) {
            // TODO @OMW there's certainly a "rewired way" of doing this
            foreach (var player in ReInput.players.Players) {
                player.controllers.maps.SetMapsEnabled(ui, "UI");
                player.controllers.maps.SetMapsEnabled(!ui, "Default");
            }
        }
    }
}