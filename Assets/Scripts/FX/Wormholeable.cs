using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame {
    [ExecuteInEditMode]
    public class Wormholeable : MonoBehaviour {
        
        [ShowInInspector, HideInEditorMode]
        private Wormhole wormhole;
        
        // TODO @OMW Remove Wormhole GameObject proxy when events allow Wormhole params
        public GameObject Wormhole {
            set { wormhole = value.GetComponent<Wormhole>(); }
        }
        
        public Renderer Renderer;

        private void Update() {
            if (wormhole) {
                foreach (var mat in Renderer.sharedMaterials) {
                    mat.SetFloat("_WormholeEnabled", 1);
                    mat.SetVector("_WormholeOrigin", wormhole.transform.position);
                    
                    mat.SetFloat("_DistortionRadius", wormhole.Radius);
                    mat.SetFloat("_DistortionPower", wormhole.Power);
            
                    mat.SetVector("_RotationAxis", wormhole.RotationAxis.normalized);
                    mat.SetFloat("_RotationSpeed", wormhole.RotationSpeed);
                    mat.SetFloat("_RotationAmplitude", wormhole.RotationAmplitude);
            
                    mat.SetFloat("_DistortionColorTreshold", wormhole.ColorTreshold);
                    mat.SetVector("_DistortionColor", wormhole.DistortionColor);
                }
            }
        }
    }
}