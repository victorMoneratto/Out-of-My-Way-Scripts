using System;
using UnityEngine.Serialization;

namespace OMWGame.Scriptable {
    [Serializable]
    public class IntReference : Reference<int, IntAsset> {
        public IntReference(int value) : base(value) { }
        
        public static implicit operator IntReference(int value) {
            return new IntReference(value);
        }
    }
}