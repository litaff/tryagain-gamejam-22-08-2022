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

        public CharacterInput()
        {
            basicInput = new Vector2();
        }

        public void SetValues(CharacterInput input)
        {
            basicInput = input.basicInput;
            jumpButtonDown = input.jumpButtonDown;
            jumpButtonUp = input.jumpButtonUp;
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
            return a.jumpButtonUp == b.jumpButtonUp;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((CharacterInput) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = basicInput.GetHashCode();
                hashCode = (hashCode * 397) ^ jumpButtonDown.GetHashCode();
                hashCode = (hashCode * 397) ^ jumpButtonUp.GetHashCode();
                return hashCode;
            }
        }
        
        private bool Equals(CharacterInput other)
        {
            return basicInput.Equals(other.basicInput) && jumpButtonDown == other.jumpButtonDown && jumpButtonUp == other.jumpButtonUp;
        }
    }
}