using OMWGame.Scriptable;
using UnityEngine;

namespace OMWGame {
    [RequireComponent(typeof(Rigidbody))]
    public class LaserMovement : MonoBehaviour {
        private Rigidbody body;

        [SerializeField]
        private FloatReference speed = 200;

        private void Awake() {
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
		    // body.angularVelocity = Vector3.zero; // Maybe?
            body.velocity = speed * (body.rotation * Vector3.forward);
        }
    }
}