using UnityEngine;

namespace _Scripts
{
    // TODO: check for null/empty instructions and take care of the game object to not create errors
    public class Afterimage : Character
    {
        private AfterimageInput[] _instructions;
        private CharacterInput _currentInput;
        private float _currentTime;
        private int _currentInstructionID;
        
        public void Init(float gravity, float maxJumpVelocity, float minJumpVelocity, AfterimageInput[] instructions)
        {
            base.Init(gravity, maxJumpVelocity, minJumpVelocity);
            _instructions = instructions;
            _currentInstructionID = 0;
            if(instructions.Length > 0)
                _currentInput = _instructions[_currentInstructionID].input;
        }

        public void UpdateCharacter()
        {
            if (_currentInstructionID + 1 >= _instructions.Length 
                && _currentTime < _instructions[_currentInstructionID].executionDuration)
            {
                Debug.Log("Afterimage dissolves");
                return;
            }
            
            if (_currentTime < _instructions[_currentInstructionID].executionDuration)
            {
                base.UpdateCharacter(_currentInput);
            
                _currentTime += Time.deltaTime;
            }
            else
            {
                _currentInstructionID++;
                _currentTime = 0;
                _currentInput = _instructions[_currentInstructionID].input;
            }

            

        }
    }
}