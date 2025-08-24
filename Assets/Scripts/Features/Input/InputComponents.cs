using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;

namespace Features.Input
{
    public struct InputBridge : IComponentData
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool isFocussed;
    }

    public struct GameCommands : IComponentData, IEquatable<GameCommands>, IEnableableComponent
    {
        private const float PressedThreshold = 0.2f;
        
        [MarshalAs(UnmanagedType.U1)]
        public bool isTargetValid;
        public float2 targetValue;
        public float2 moveValue;
        
        public ActionButton feed;
        public ActionButton jump;
        public ActionButton pause;
        
        public void ResetFrameEvents()
        {
            feed.ResetFrameEvents();
            jump.ResetFrameEvents();
            pause.ResetFrameEvents();
        }
        
        public bool Equals(GameCommands other)
        {
            return moveValue.Equals(other.moveValue) 
                   && targetValue.Equals(other.targetValue) 
                   && feed.Equals(other.feed)
                   && jump.Equals(other.jump)
                   && pause.Equals(other.pause);
        }

        public override bool Equals(object obj)
        {
            return obj is GameCommands other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(moveValue, targetValue, feed, jump, pause);
        }
        
        public static bool operator ==(GameCommands left, GameCommands right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(GameCommands left, GameCommands right)
        {
            return !left.Equals(right);
        }
    }
}