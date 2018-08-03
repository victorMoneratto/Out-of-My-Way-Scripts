using System;
using UnityEngine;

namespace OMWGame.Scriptable {
    [Serializable]
    public class Reference<T, TAsset> where TAsset : CustomAsset<T> {
        public bool UseConstant = true;
        public T ConstantValue;
        public TAsset Asset;

        public Reference(T value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public T Value => UseConstant ? ConstantValue : Asset.Value;

        public static implicit operator T(Reference<T, TAsset> reference) {
            return reference.Value;
        }
        
        
    }
}