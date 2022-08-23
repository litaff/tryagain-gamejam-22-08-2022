using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _Scripts
{
    public class Player : Character
    {
        private CharacterInput _playerInput;
        private CharacterInput _oldPlayerInput;
        public static event Action<CharacterInput> OnCharacterInputChange;
        
        protected override void Start()
        {
            base.Start();
            _playerInput = new CharacterInput();
            _oldPlayerInput = new CharacterInput();
        }
        
        private void Update()
        {
            _playerInput.basicInput =new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _playerInput.jumpButtonUp = Input.GetKeyUp(KeyCode.Space);
            _playerInput.jumpButtonDown = Input.GetKeyDown(KeyCode.Space);

            if (_playerInput != _oldPlayerInput)
            {
                OnCharacterInputChange?.Invoke(_oldPlayerInput);
            }
                
            
            UpdateCharacter(_playerInput);
            _oldPlayerInput.SetValues(_playerInput);
        }
        
    }
}