using System.Collections;
using OMWGame.Extensions;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    public class Shield : MonoBehaviour {

        [ReadOnly, HideInEditorMode]
        [ShowInInspector] private bool isRaised = false;
        
        [SerializeField] private FloatReference scale = 2f;


        #region Curves
        [SerializeField] private AnimationCurve raiseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve dropCurve  = AnimationCurve.EaseInOut(0, 1, 1, 0);
        #endregion


        #region Events
        [SerializeField] private UnityEvent raisedEvent;
        [SerializeField] private UnityEvent dropEvent;
        #endregion

        private void Awake() {
            // @OMW not sure we should do this here, but dont mind rn
            transform.localScale = Vector3.zero;
        }

        private void OnDisable() {
            bool wasRaised = isRaised;
            isRaised = false;
            
            // @OMW invoke dropEvent if disabled while raised?
            if (wasRaised) {
                // should I?
                // dropEvent.Invoke();
            }
        }


        public void Raise(float duration) {
            isRaised = true;
            raisedEvent.Invoke();
            
            StartCoroutine(ShieldRaised(duration));
        }

        private IEnumerator ShieldRaised(float duration) {
            float raiseTime = raiseCurve.Time();
            float dropTime  = dropCurve.Time();

            float time = 0;
            while (time < duration) {
                float timeleft = duration - time;

                float factor = 0f;
                if (time <= raiseTime) {
                    // raise
                    factor = raiseCurve.Evaluate(time);
                    
                } else if (timeleft > dropTime) {
                    // keep up
                    factor = 1f;
                    
                } else {
                    // drop
                    float t = time - (duration - dropTime);
                    factor = dropCurve.Evaluate(t);
                }
                
                transform.localScale = Vector3.one * factor * scale;
                yield return null;

                time += Time.deltaTime;
            }

            isRaised = false;
            dropEvent.Invoke();
        }
    }
}