using System;
using UnityEngine;

namespace OMWGame {
    [RequireComponent(typeof(TeamComponent))]
    public class LaserColorChanger : MonoBehaviour {
        [Serializable]
        public class ParticleGradient {
            public ParticleSystem Particle;

            [NonSerialized]
            public Gradient Gradient;
        }

        public ParticleGradient Glow;
        public ParticleGradient Outer;

        private void Start() {
            var team = GetComponent<TeamComponent>();
            if (team) {
                Glow.Gradient = team.Team.LaserGlowGradient;
                Outer.Gradient = team.Team.LaserOuterGradient;
            }

            var gradients = new[] {Outer, Glow};
            for (int i = 0; i < gradients.Length; i++) {
                var col = gradients[i].Particle.colorOverLifetime;
                col.color = gradients[i].Gradient;
            }
        }
    }
}