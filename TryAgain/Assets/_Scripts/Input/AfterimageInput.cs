using System;
using UnityEngine;

namespace _Scripts
{
    [Serializable]
    public struct AfterimageInput
    {
        public CharacterInput input;
        public float executionDuration;

        public AfterimageInput(CharacterInput input, float time)
        {
            this.input = new CharacterInput();
            this.input.SetValues(input);
            executionDuration = time;
        }
    }
}