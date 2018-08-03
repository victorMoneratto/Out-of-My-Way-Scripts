using System;
using System.Collections;
using OMWGame.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    public class Gate : MonoBehaviour {

        [SerializeField] private AnimationCurve openGateDissolve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve closeGateDissolve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Serializable] class GateEvent : UnityEvent<GameObject> { }
        [SerializeField] private GateEvent openEvent;
        [SerializeField] private GateEvent closeEvent;

        private Material material;

        public void Awake() {
            material = GetComponent<Renderer>().material;
        }
        
        public void Open() {
            openEvent.Invoke(gameObject);
            StartCoroutine(Tween(openGateDissolve));
        }

        public void Close() {
            closeEvent.Invoke(gameObject);
            StartCoroutine(Tween(closeGateDissolve));
        }

        private IEnumerator Tween(AnimationCurve curve) {
            float time = 0;
            float maxTime = curve.Time();
            while (time < maxTime) {
                var dissolve = curve.Evaluate(time); 
                material.SetFloat("_Dissolve", dissolve);
                yield return null;
                time += Time.deltaTime;
            }

            material.SetFloat("_Dissolve", curve.Evaluate(maxTime));
        }
    }
}