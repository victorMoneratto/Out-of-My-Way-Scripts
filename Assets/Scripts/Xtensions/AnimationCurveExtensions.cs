using UnityEngine;

namespace OMWGame.Extensions {
    public static class AnimationCurveExtensions {
        
        /// <summary>
        /// Time of the last key
        /// </summary>
        public static float Time(this AnimationCurve curve) {
            int lastKey = curve.keys.Length - 1;
            return (lastKey > 0 ? curve.keys[lastKey].time : 0);
        }
    }
}