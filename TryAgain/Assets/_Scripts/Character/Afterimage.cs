using System;
using _Scripts.Manager;
using UnityEngine;

namespace _Scripts
{
    public class Afterimage : Character
    {
        [SerializeField] private Animator weaponAnimator;
        private AfterimageInput[] _instructions;
        private float _currentTime;
        private int _currentInstructionID;
        
        public static event Action<Npc> OnNpcKill;
        
        public void Init(float gravity, float maxJumpVelocity, float minJumpVelocity, AfterimageInput[] instructions)
        {
            base.Init(gravity, maxJumpVelocity, minJumpVelocity);
            _instructions = instructions;
            _currentInstructionID = 0;
            if(instructions.Length > 0)
                CurrentInput = _instructions[_currentInstructionID].input;
        }

        public override void UpdateCharacter()
        {
            weaponAnimator.SetBool("Attack", false);
            if (Dead) return; // char dead
            if (_instructions.Length < 1 || _currentInstructionID + 1 >= _instructions.Length 
                && _currentTime < _instructions[_currentInstructionID].executionDuration)
            {
                Debug.Log("Afterimage dissolves");
                base.UpdateCharacter();
                
                if (CurrentInput.attackDirection == Vector2.zero) return;
                Attack();
                
                return;
            }
            
            if (_currentTime < _instructions[_currentInstructionID].executionDuration)
            {
                base.UpdateCharacter();
                if (CurrentInput.attackDirection != Vector2.zero)
                {
                    Attack();
                }
                _currentTime += TimeManager.TimePassedFor(this);
            }
            else
            {
                _currentInstructionID++;
                _currentTime = 0;
                CurrentInput = _instructions[_currentInstructionID].input;
            }
        }

        private void Attack()
        {
            weaponAnimator.SetBool("Attack", true);
            var degree = 
                Mathf.Abs(90 - CurrentInput.attackDirection.x * 90 - CurrentInput.attackDirection.y * 180);
            attackDetector.RotateDetection(degree);
            var npcs = attackDetector.GetAllOfType<Npc>();
            foreach (var npc in npcs)
            {
                OnNpcKill?.Invoke(npc);
            }
        }
    }
}