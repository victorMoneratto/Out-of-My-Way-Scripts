using System;

namespace OMWGame {
    [Serializable]
    public class PIDController {
        public PIDControllerConfig Data;

        private float lastError = 0f;
        private float integralError = 0f;

        public float Update(float error, float dt) {
            var d = (error - lastError) / dt;
            integralError += error * dt;
            lastError = error;

            return Data.P * error + Data.I * integralError + Data.D * d;
        }
    }
}