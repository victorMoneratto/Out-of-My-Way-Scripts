using OMWGame.Scriptable;
using UnityEngine;

namespace OMWGame {
    public class SpacecraftPreparation : MonoBehaviour {
        [SerializeField] private GameObjectRunSet alivePlayers;


        public void BeginPrepare(float time) {
            foreach (var player in alivePlayers) {
                var input = player.GetComponent<PlayerInput>();
                input.InputEnabled = false;
                
                var shield = player.GetComponentInChildren<Shield>();
                if (shield) {
                    shield.Raise(time);
                }
            }
        }

        public void EndPrepare() {
            foreach (var player in alivePlayers) {
                var input = player.GetComponent<PlayerInput>();
                input.InputEnabled = true;
            }
        }
    }
}