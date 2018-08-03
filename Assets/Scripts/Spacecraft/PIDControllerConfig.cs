using UnityEngine;

namespace OMWGame {
    [CreateAssetMenu(fileName = "PID", menuName = "Config/PID Controller")]
    public class PIDControllerConfig : ScriptableObject {
        public float P = 1;
        public float I = 0;
        public float D = 0.1f;
    }
}