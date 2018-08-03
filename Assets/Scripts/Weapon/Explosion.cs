using UnityEngine;

namespace OMWGame {
    public class Explosion : MonoBehaviour {
        [SerializeField] private GameObject fx;

        public void Explode() {
            // @OMW TODO pooling
            Instantiate(fx, transform.position, transform.rotation);
            
            // @OMW TODO random rotation
        }
    }
}