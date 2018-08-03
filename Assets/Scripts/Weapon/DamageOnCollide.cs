using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    [RequireComponent(typeof(Rigidbody))]
    public class DamageOnCollide : MonoBehaviour, IDamager {
        #region IDamager
        [ReadOnly, HideInEditorMode]
        [ShowInInspector]
        public GameObject Instigator { get; set; }
        #endregion


        public int Damage = 1;
        [SerializeField] private bool autoDestroy = true;
        [SerializeField] private bool ignoreInstigator = true;
        [SerializeField] private GameObject impact;

        [Serializable] class DamageOnDestroyEvent : UnityEvent<GameObject> { }
        [SerializeField] private DamageOnDestroyEvent damageDeltEvent;


        #region Query
        public bool DoQuery = true;

        [ShowIf("DoQuery")]
        public float QueryRadius = 1f;
        #endregion


        private Vector3 lastPosition;

        private void OnEnable() {
            lastPosition = transform.position;
        }

        private void Update() {
            // Auxiliary casting for collision in case the rigidbody
            // solution is not suitable
            if (DoQuery) {
                Vector3 origin = lastPosition;
                Vector3 end = transform.position;
                Vector3 offset = end - origin;
                float dist = offset.magnitude;

                if (dist > 0) {
                    Vector3 dir = offset.normalized;
                    RaycastHit hit;
                    if (Physics.SphereCast(origin, QueryRadius, dir, out hit, dist) ||
                        Physics.SphereCast(end, QueryRadius, -dir, out hit, dist)) {
                        DoDamage(hit.transform.GetComponent<Health>());
                        // Debug.DrawLine(hit.point, hit.point + 2 * hit.normal, Color.green, 10f);
                    }

                    // Debug.DrawLine(origin, end, Color.red, 2 * Time.deltaTime);
                }
            }

            lastPosition = transform.position;
        }

        private void OnCollisionEnter(Collision other) {
            DoDamage(other.gameObject.GetComponent<Health>());
        }

        private void DoDamage(Health health) {
            // @OMW TODO Rules are still unclear here, leading to poor code
            if (health && health.gameObject == Instigator && ignoreInstigator) {
                return;
            }

            if (health) {
                int damageDelt = health.ReceiveDamage(Damage, this);
                if (damageDelt > 0) {
                    damageDeltEvent.Invoke(gameObject);
                }
            }

            if (autoDestroy) {
                
                
                // TODO @OMW Find a way to handle these things below to the event
                // TODO @OMW support pooling for both impact and gameObject
                Destroy(gameObject);
                Instantiate(impact, transform.position, transform.rotation);
            }
        }

#if false
        private void OnDrawGizmos() {
            if (DoQuery) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(lastPosition, QueryRadius);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, QueryRadius);
            }
        }
        #endif
    }
}