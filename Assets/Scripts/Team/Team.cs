using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame {
    [Serializable]
    public class Team {
        public string Name = "";

        public Color Color = Color.white;
        public Material Material;

        public Dictionary<string, Gradient> Colors = new Dictionary<string, Gradient>();

        #region Laser
        [Title("Laser Projectile")]
        public Gradient LaserGlowGradient;
        public Gradient LaserOuterGradient;
        
        [Title("Laser Muzzle")]
        public Gradient LaserMuzzleConeGradient;
        public Gradient LaserMuzzleSpriteGradient;
        public Gradient LaserMuzzleGlowGradient;
        #endregion

        #region Equality
        
        public static bool operator ==(Team lhs, Team rhs) {
            if (ReferenceEquals(lhs, null)) {
                if (ReferenceEquals(rhs, null)) {
                    return true;
                }
                return false;
            }
            
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Team lhs, Team rhs) {
            return !(lhs == rhs);
        }

        protected bool Equals(Team other) {
            // null-check needed because Sirenix Odin
            // calls it directly inside the editor 
            if (ReferenceEquals(other, null)) {
                return false;
            }
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Team) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (base.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        #endregion
    }
}