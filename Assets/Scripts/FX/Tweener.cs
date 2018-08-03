using System;
using OMWGame.Extensions;
using UnityEngine;

namespace OMWGame {
    public class Tweener : MonoBehaviour {
        [Serializable]
        public class TweenOnAxis {
            public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            
            [Flags]
            public enum Axis {
                X = 1, Y = 2, Z = 4
            }

            public Axis Axes;
        }
        
        [SerializeField] private bool tweenOnAwake = true;
        
        [SerializeField] private TweenOnAxis[] positionTweens;
        [SerializeField] private TweenOnAxis[] rotationTweens; // in "euler space"
        [SerializeField] private TweenOnAxis[] scaleTweens;
        
        public enum Spaces {
            Local, World, UI
        }

        [SerializeField] private Spaces space = Spaces.Local;

        private float startTime = 0;
        private RectTransform rect;

        public void Play() {
            startTime = Time.time;
        }

        public void Stop() {
            startTime = 0;
        }

        private void Awake() {
            if (tweenOnAwake) {
                Play();
            }

            rect = GetComponent<RectTransform>();
        }

        private void LateUpdate() {
            if (startTime > 0) {
                switch (space) {
                    case Spaces.Local:
                        transform.localPosition = TweenVector3(transform.localPosition, positionTweens);
                        transform.localEulerAngles = TweenVector3(transform.localEulerAngles, positionTweens);
                        transform.localScale = TweenVector3(transform.localScale, positionTweens);
                        break;
                    
                    case Spaces.World:
                        transform.position = TweenVector3(transform.position, positionTweens);
                        transform.eulerAngles = TweenVector3(transform.eulerAngles, positionTweens);
                        // scaling on world space is problematic and lossy, we'll scale on local space only 
                        transform.localScale = TweenVector3(transform.localScale, positionTweens);
                        break;
                    
                    case Spaces.UI:
                        rect.anchoredPosition3D = TweenVector3(rect.anchoredPosition3D, positionTweens);
                        rect.localEulerAngles = TweenVector3(rect.localEulerAngles, rotationTweens);
                        rect.localScale = TweenVector3(rect.localScale, scaleTweens);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Vector3 TweenVector3(Vector3 vector, TweenOnAxis[] tweens) {
            foreach (var tween in tweens) {
                float value = tween.Curve.Evaluate(Time.time - startTime);
                if (tween.Axes.HasFlag(TweenOnAxis.Axis.X)) {
                    vector.x = value;
                }

                if (tween.Axes.HasFlag(TweenOnAxis.Axis.Y)) {
                    vector.y = value;
                }

                if (tween.Axes.HasFlag(TweenOnAxis.Axis.Z)) {
                    vector.z = value;
                }
            }

            return vector;
        }
    }
}