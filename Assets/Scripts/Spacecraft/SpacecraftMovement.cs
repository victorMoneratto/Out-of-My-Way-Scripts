using System.Collections;
using OMWGame.Scriptable;
using UnityEngine;

namespace OMWGame {
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Rigidbody))]
    public class SpacecraftMovement : MonoBehaviour {
        #region Translation and Rotation
        [SerializeField] private FloatReference acceleration = 70;
        [SerializeField] private PIDController rotationPID;
        [SerializeField] private FloatReference highDrag = 7;
        #endregion


        #region Boost
        [SerializeField] private FloatReference boostCooldown = 1;
        [SerializeField] private FloatReference boostMaxTime = 1;
        [SerializeField] private FloatReference boostVelocity = 100;

        private bool canBoost   = true;
        private bool isBoosting = false;
        #endregion
        
        
        #region Components
        private PlayerInput input;
        private Rigidbody   body;
        #endregion

        private void Awake() {
            input = GetComponent<PlayerInput>();
            body  = GetComponent<Rigidbody>();
        }
        
        private void FixedUpdate() {
            // Input
            Vector3 moveDir = Vector3.zero;
            bool wantsBoost = false;
            if (input.Control != null) {
                moveDir.x  = input.Control.GetAxis("Move Horizontal");
                moveDir.y  = input.Control.GetAxis("Move Vertical");
                moveDir.z = 0;
                
                wantsBoost = input.Control.GetButton("Boost");
            }
            
            // Boosting
            if (wantsBoost && canBoost) {
                StartCoroutine(Boost());
            }

            if (!isBoosting) {
                // Translation
                body.AddForce(moveDir * acceleration);

                // Rotation
                if (moveDir.sqrMagnitude > 0f) {
                    // free-up when controlling
                    body.angularDrag = 0;

                    Vector3 fwd     = transform.forward;
                    float   current = Mathf.Atan2(-fwd.y,     fwd.x)     * Mathf.Rad2Deg;
                    float   target  = Mathf.Atan2(-moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                    float   delta   = rotationPID.Update(Mathf.DeltaAngle(current, target), Time.deltaTime);
                    body.AddRelativeTorque(Vector3.up * delta, ForceMode.Acceleration);

                    #if false
                    Grapher.Log(target, "Target");
                    Grapher.Log(current, "Current");
                    Grapher.Log(delta, "Delta");
                    #endif
                } else {
                    // @OMW TODO check for straying before giving control back
                    body.angularDrag = highDrag;
                }
            } else {
                body.angularVelocity = Vector3.zero;
                body.velocity        = transform.forward * boostVelocity;
            }
        }

        private IEnumerator Boost() {
            isBoosting = true;
            canBoost   = false;

            float start = Time.time;
            while (Time.time < start + boostMaxTime) {
                if (!isBoosting) {
                    // something stopped us from boosting
                    break;
                }

                yield return null;
            }

            isBoosting = false;

            yield return new WaitForSeconds(boostCooldown);
            canBoost = true;
        }

        private void OnCollisionEnter(Collision other) {
            if (StopBoosting()) { }
        }

        private void OnCollisionStay(Collision other) {
            if (StopBoosting()) { }
        }

        private bool StopBoosting() {
            if (isBoosting) {
                isBoosting = false;
                return true;
            }

            return false;
        }
    }
}