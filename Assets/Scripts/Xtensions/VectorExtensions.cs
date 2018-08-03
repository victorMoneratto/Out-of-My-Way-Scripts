﻿using UnityEngine;

namespace OMWGame.Extensions {
    public static class VectorExtensions {
        public static Vector3 WithX(this Vector3 v, float x) {
            return new Vector3(x, v.y, v.z);
        }
        
        public static Vector3 WithY(this Vector3 v, float y) {
            return new Vector3(v.x, y, v.z);
        }
        
        public static Vector3 WithZ(this Vector3 v, float z) {
            return new Vector3(v.x, v.y, z);
        }
    }
}