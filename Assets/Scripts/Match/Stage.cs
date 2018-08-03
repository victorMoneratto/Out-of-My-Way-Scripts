using System;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    public class Stage : MonoBehaviour {
        
        #region Gate
        public StageSafeZone SafeZone;
        
        [SerializeField] private Gate leftGate;
        [SerializeField] private Gate rightGate;

        public void OpenGate(Side side) {
            Gate gate;
            switch (side) {
                case Side.Left:
                    gate = leftGate;
                    break;
                case Side.Right:
                    gate = rightGate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            gate.Open();
        }

        public void CloseGate(Side side) {
            Gate gate;
            switch (side) {
                case Side.Left:
                    gate = leftGate;
                    break;
                case Side.Right:
                    gate = rightGate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            gate.Close();
        }
        #endregion
        
        public PlayerSpawner[] Spawners;
        
        [NonSerialized] public GameObject[] CraftsFromThisStage;
        
        
        // TODO @OMW refactor ending/special stage stuff, both here and in MatchMode
        
        // On special stages (like wormhole ending), no ships should be spawned
        public bool EndingStage = false;
    }
}