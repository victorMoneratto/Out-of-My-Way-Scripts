using System;

namespace OMWGame {
    public enum Side {
        Left, Right
    }

    public static class SideExtensions {
        public static Side GetOpposite(this Side side) {
            switch (side) {
                case Side.Left: return Side.Right;
                case Side.Right: return Side.Left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
    }
}