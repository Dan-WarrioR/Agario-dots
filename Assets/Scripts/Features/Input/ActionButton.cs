using System;
using System.Runtime.InteropServices;

namespace Features.Input
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ActionButton : IEquatable<ActionButton>
    {
        private const float PressedThreshold = 0.2f;
        
        public float rawValue;
        [MarshalAs(UnmanagedType.U1)] public bool down; 
        
        public bool IsPressed => rawValue > PressedThreshold;

        public void Update(float newValue)
        {
            bool wasPressed = IsPressed;
            rawValue = newValue;
            bool isPressed = IsPressed;
            down = !wasPressed && isPressed;
        }
        
        public void ResetFrameEvents()
        {
            down = false;
        }

        public bool Equals(ActionButton other)
        {
            return rawValue.Equals(other.rawValue) && down == other.down;
        }

        public override bool Equals(object obj)
        {
            return obj is ActionButton other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(rawValue, down);
        }

        public static bool operator ==(ActionButton left, ActionButton right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ActionButton left, ActionButton right)
        {
            return !left.Equals(right);
        }
    }
}