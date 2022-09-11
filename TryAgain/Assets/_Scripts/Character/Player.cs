using System;
using UnityEngine;

namespace _Scripts
{
    // TODO: handle the attack line 39
    public class Player : Character
    {
        [SerializeField] private Animator weaponAnimator;
        private CharacterInput _oldPlayerInput;

        public static event Action<CharacterInput> OnCharacterInputChange;
        public static event Action<Npc> OnNpcKill;

        public void Init(float gravity, float maxJumpVelocity, float minJumpVelocity, Vector2 startPos)
        {
            base.Init(gravity, maxJumpVelocity, minJumpVelocity);
            Dead = false;
            transform.position = startPos;
        }
        
        protected override void Start()
        {
            base.Start();
            _oldPlayerInput = new CharacterInput();
            Melee.OnPlayerKill += Die;
        }
        
        private void Update()
        {
            weaponAnimator.SetBool("Attack", false);
            if (Dead) return; // char dead
            CurrentInput.basicInput = new Vector2(
                Input.GetAxisRaw("Horizontal"), 
                Input.GetAxisRaw("Vertical"));
            CurrentInput.jumpButtonUp = Input.GetKeyUp(KeyCode.Space);
            CurrentInput.jumpButtonDown = Input.GetKeyDown(KeyCode.Space);
            CurrentInput.attackDirection = new Vector2(
                Input.GetAxisRaw("Horizontal Attack"), 
                Input.GetAxisRaw("Vertical Attack"));

            if (CurrentInput.attackDirection != Vector2.zero)
            {
                Attack();
            }
            
            if (CurrentInput != _oldPlayerInput)
            {
                OnCharacterInputChange?.Invoke(_oldPlayerInput);
            }
                
            
            UpdateCharacter();
            _oldPlayerInput.SetValues(CurrentInput);
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