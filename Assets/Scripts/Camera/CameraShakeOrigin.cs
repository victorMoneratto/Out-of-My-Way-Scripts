using System;
using Cinemachine;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace OMWGame {
    public class CameraShakeOrigin : MonoBehaviour {

        [SerializeField] private NoiseSettings noise;
        
        // Greater takes priority
        public int Priority = 0;

        public AnimationCurve GainCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        /// Multiply by GameObject forward direction (see GetNoiseProfile) 
        [SerializeField] private bool modulateByOrientation;
        
        [ShowIf("modulateByOrientation")]
        [SerializeField] private Vector3 modulationMin = new Vector3(.1f, .1f);
        
        // can be replaced with UnityEvent but I feel it would just hide other
        // logic that shouldn't be in here
        [SerializeField] private GameObjectEvent shakeEvent;

        public void Shake() {
            if (shakeEvent) {
                shakeEvent.Raise(gameObject);
            }
        }

        public NoiseSettings GetNoiseProfile() {
            if (modulateByOrientation) {
                // TODO @OMW take away garbage instancing and hardcoded copying of data
                var modulated = ScriptableObject.CreateInstance<NoiseSettings>();
                modulated.PositionNoise = new NoiseSettings.TransformNoiseParams[modulated.PositionNoise.Length];
                Array.Copy(noise.PositionNoise, modulated.PositionNoise, modulated.PositionNoise.Length);
                modulated.OrientationNoise = new NoiseSettings.TransformNoiseParams[modulated.OrientationNoise.Length];
                Array.Copy(noise.OrientationNoise, modulated.OrientationNoise, modulated.OrientationNoise.Length);

                var modulation = Vector3.Max(transform.forward.Abs(), modulationMin);

                for (int i = 0; i < modulated.PositionNoise.Length; i++) {
                    modulated.PositionNoise[i].X.Amplitude *= modulation.x;
                    modulated.PositionNoise[i].Y.Amplitude *= modulation.y;
                    modulated.PositionNoise[i].Z.Amplitude *= modulation.z;
                }

                return modulated;
            }

            return noise;
        }
    }
}