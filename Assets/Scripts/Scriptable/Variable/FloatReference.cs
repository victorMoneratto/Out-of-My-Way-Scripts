using System;

namespace OMWGame.Scriptable {
    [Serializable]
    public class FloatReference : Reference<float, FloatAsset> {
        public FloatReference(float value) : base(value) { }
        
        public static implicit operator FloatReference(float value) {
            return new FloatReference(value);
        }
    }
}