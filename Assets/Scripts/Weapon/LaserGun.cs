using UnityEngine;

namespace OMWGame {
    public class LaserGun : MonoBehaviour {
        
        [SerializeField] private ParticleSystem particle;
        
        public void Play() {
            particle.Play(true);
        }
    }
}