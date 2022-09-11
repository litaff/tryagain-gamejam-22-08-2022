using System;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace _Scripts
{
    [Serializable]
    public class CharacterInput
    {
        /// <summary>
        /// WSAD input
        /// </summary>
        public Vector2 basicInput;
        /// <summary>
        /// Jump button has been pressed
        /// </summary>
        public bool jumpButtonDown;
        /// <summary>
        /// Jump button has been released
        /// </summary>
        public bool jumpButtonUp;
        /// <summary>
        /// Arrow input
        /// </summary>
        public Vector2 attackDirection;

        public CharacterInput()
        {
            basicInput = new Vector2();
        }

        public void SetValues(CharacterInput input)
        {
            basicInput = input.basicInput;
            jumpButtonDown = input.jumpButtonDown;
            jumpButtonUp = input.jumpButtonUp;
            attackDirection = input.attackDirection;
        }
        
        public static bool operator !=(CharacterInput a, CharacterInput b)
        {
            if (a is null || b is null) return false;
            
            return !(a == b);
        }

        public static bool operator ==(CharacterInput a, CharacterInput b)
        {
            if (a is null || b is null) return false;
            
            if (a.basicInput != b.basicInput)
                return false;
            if (a.jumpButtonDown != b.jumpButtonDown)
                return false;
            if (a.jumpButtonUp != b.jumpButtonUp)
                return false;
            return a.attackDirection == b.attackDirection;
        }
    }
}