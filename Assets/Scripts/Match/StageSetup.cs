using System;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame {
    [CreateAssetMenu(fileName = "Stage", menuName = "Stage")]
    public class StageSetup : ScriptableObject {
        [Required]
        public Stage Object;

        
        [Serializable]
        public class Arsenal {
            public TeamAsset Team;
            public GameObject[] Crafts;
        }

        public Arsenal[] Arsenals;
    }
}