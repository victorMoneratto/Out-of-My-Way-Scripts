using System;
using System.Collections.Generic;
using OMWGame.Scriptable;
using UnityEngine;

namespace OMWGame {
    
    [Serializable]
    public class PlayerState {
        public int InputID;
        public TeamAsset Team;
    }
    
    [CreateAssetMenu(fileName = "Match Setup", menuName = "Match Setup")]
    public class PlayersInMatch : RuntimeSet<PlayerState> { }
}