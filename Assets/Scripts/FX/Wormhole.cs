using System.Collections.Generic;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame {
    public class Wormhole : MonoBehaviour {
        
        
        #region Material
        public float Radius = 5;
        public float Power = 2;
        public Vector3 RotationAxis = new Vector3(0, 0, 1);
        public float RotationSpeed = 10;
        public float RotationAmplitude = .1f;
        public float ColorTreshold = .25f;

        [ColorUsage(showAlpha: false, hdr: true)]
        public Color DistortionColor = Color.yellow;
        #endregion


        #region Trap
        [ShowInInspector, HideInEditorMode]
        private List<Rigidbody> trappedObjects = new List<Rigidbody>();

        [SerializeField]
        private float force = 10;
        
        public GameObjectEvent ActivateWormholeEvent;
        public GameObjectEvent PlayerTrappedEvent;
        #endregion
        
        
        public void ActivateWormhole() {
            if (ActivateWormholeEvent) {
                ActivateWormholeEvent.Raise(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other) {
            var body = other.GetComponent<Rigidbody>();
            if (body && !trappedObjects.Contains(body)) {
                trappedObjects.Add(body);

                var player = other.GetComponent<PlayerInput>();
                if (player) {
                    player.InputEnabled = false;
                    PlayerTrappedEvent.Raise(player.gameObject);
                }
            }
        }

        private void FixedUpdate() {
            foreach (var player in trappedObjects) {
                var body = player.GetComponent<Rigidbody>();

                // TODO @OMW replace with PID per player
                var vector = force * (transform.position - player.transform.position);
                body.AddForce(vector);
                body.AddRelativeTorque(force * Vector3.up);
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}