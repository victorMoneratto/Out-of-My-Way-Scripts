using Cinemachine;
using OMWGame.Extensions;
using UnityEngine;

namespace OMWGame {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraShaker : MonoBehaviour {
        
        private CinemachineBasicMultiChannelPerlin noise;
        
        public void Start() {
            var cm = GetComponent<CinemachineVirtualCamera>();
            noise = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        
        #region Runtime shake data
        private int priority;
        private float noiseTime;
        private AnimationCurve gainCurve;
        private float curveMaxTime;
        private NoiseSettings CurrentNoise => noise ? noise.m_NoiseProfile : null;
        #endregion
        
        // TODO @OMW Replace with CameraShakeOrigin parameter (parameterized events)
        public void Shake(GameObject originGo) {
            var origin = originGo.GetComponent<CameraShakeOrigin>();
            
            // if were in a higher priority shake, ignore this request
            if (CurrentNoise && priority > origin.Priority) { return; }

            priority = origin.Priority;
            noiseTime = 0;
            gainCurve = origin.GainCurve;
            noise.m_AmplitudeGain = gainCurve.Evaluate(0);
            curveMaxTime = gainCurve.Time();
            
            noise.m_NoiseProfile = origin.GetNoiseProfile();
        }

        private void Update() {
            if (noise.m_NoiseProfile) {
                // "disable" shake 1 frame after noiseTime
                // reaches end of curve
                if (noiseTime >= curveMaxTime) {
                    noise.m_NoiseProfile = null;
                    return;
                }
                
                noiseTime = Mathf.Min(noiseTime + Time.deltaTime, curveMaxTime);
                noise.m_AmplitudeGain = gainCurve.Evaluate(noiseTime);
            }
        }
    }
}