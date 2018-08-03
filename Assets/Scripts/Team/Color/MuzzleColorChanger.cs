using System;
using UnityEngine;

namespace OMWGame {
    [RequireComponent(typeof(TeamComponent))]
    public class MuzzleColorChanger : MonoBehaviour {
        [Serializable]
        public class ParticleGradient {
            public ParticleSystem Particle;

            [NonSerialized]
            public Gradient Gradient;
        }

        public ParticleGradient Cone;
        public ParticleGradient Sprite;
        public ParticleGradient Glow;

        private void Start() {
            var team = GetComponent<TeamComponent>();
            if (team) {
                Cone.Gradient = team.Team.LaserMuzzleConeGradient;
                Sprite.Gradient = team.Team.LaserMuzzleSpriteGradient;
                Glow.Gradient = team.Team.LaserMuzzleGlowGradient;
            }

            var gradients = new[] {Cone, Sprite, Glow};
            for (int i = 0; i < gradients.Length; i++) {
                var col = gradients[i].Particle.colorOverLifetime;
                col.color = gradients[i].Gradient;
            }
        }
    }
}