using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame {
    public class PlayerInput : MonoBehaviour {
        [SerializeField, HideInEditorMode]
        public int PlayerID = 0;

        public bool InputEnabled = true;

        // @OMW TODO Cache control
        public Player Control => ReInput.isReady && InputEnabled ? ReInput.players.GetPlayer(PlayerID) : null;
    }
}