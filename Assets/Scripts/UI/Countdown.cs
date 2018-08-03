using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    public class Countdown : MonoBehaviour {
        [SerializeField] private float time = 3;

        #if UNITY_EDITOR
        [SerializeField] private bool skipInEditor = true;
        #endif

        [Serializable] class CountdownStartEvent : UnityEvent<float> { }
        [SerializeField] private CountdownStartEvent start;
        
        [Serializable] class CountdownTickEvent : UnityEvent<string> { }
        [SerializeField] private CountdownTickEvent tick;
        
        [SerializeField] private UnityEvent go;
        
        [SerializeField] private UnityEvent finish;

        public void StartCountdown() {
            #if UNITY_EDITOR
            if (skipInEditor) { return; }
            #endif
            
            StartCoroutine(DoCountdown(time));
            start.Invoke(time);
        }

        private IEnumerator DoCountdown(float timeLeft) {
            float tickPeriod = 1;
            while (timeLeft > 0) {
                tick.Invoke(timeLeft.ToString(CultureInfo.InvariantCulture));
                yield return new WaitForSeconds(tickPeriod);
                timeLeft -= tickPeriod;
            }
            
            go.Invoke();
            
            yield return new WaitForSeconds(tickPeriod);
            finish.Invoke();
        }
    }
}