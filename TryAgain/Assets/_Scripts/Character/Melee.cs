using System;
using System.Collections;
using System.Linq;
using _Scripts.Manager;
using UnityEngine;

namespace _Scripts
{
    public class Melee : Npc
    {
        [SerializeField] private float patrolSpeed;
        [SerializeField] private float combatSpeed;
        [SerializeField] private Detector agroDetector;
        [SerializeField] private Animator animator;
        private bool _combatMode;
        private bool _attacking;
        private Player _player;
        private float _patrolTimer;

        public static event Action OnPlayerKill;
        
        protected override void Start()
        {
            base.Start();
            _patrolTimer = 0;
            _combatMode = false;
            _attacking = false;
            agroDetector.OnCharacterEnter += EnterCombat;
        }

        public override void Act()
        {
            if (Dead) return; // char dead

            if(_combatMode)
                Combat();
            else
                Patrol();
            
            UpdateCharacter();
        }

        public override void Die()
        {
            animator.SetBool("Attack", false);
            base.Die();
        }

        // random movement left/right
        private void Patrol()
        {
            moveSpeed = patrolSpeed;
            _patrolTimer -= TimeManager.TimePassedFor(this);
            if (_patrolTimer <= 0)
            {
                ChangeDirection();
            }
            else if (_patrolTimer <= 1 && _patrolTimer > 0)
            {
                CurrentInput.basicInput = new Vector2(
                    0f, CurrentInput.basicInput.y);
            }
        }
        
        private void Combat()
        {
            if (_attacking) return;
            animator.SetBool("Attack", false);
            var playerPosition = _player.transform.position;
            var position = transform.position;
            var delta = playerPosition.x - position.x;
            var dir = (playerPosition - position).normalized;
            moveSpeed = combatSpeed;
            if (delta > 2f || delta < -2f) // out of range
            {
                CurrentInput.basicInput = new Vector2(
                    Mathf.Sign(delta), CurrentInput.basicInput.y);
                
            }
            else // in range
            {
                CurrentInput.basicInput = new Vector2(
                    0f, CurrentInput.basicInput.y);
                CurrentInput.attackDirection = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y) 
                    ? new Vector2(Mathf.Sign(dir.x), 0f) 
                    : Vector2.zero;
                _attacking = true;
                StartCoroutine(AttackSeq(1f));
            }
            
        }

        private IEnumerator AttackSeq(float time)
        {
            yield return new WaitForSeconds(time);
            if(!Dead)
                animator.SetBool("Attack", true);
            yield return new WaitForSeconds(0.1f);
            if(!Dead)
                Attack();
            _attacking = false;
        }
        
        private void Attack()
        {
            var degree = 
                Mathf.Abs(90 - CurrentInput.attackDirection.x * 90 - CurrentInput.attackDirection.y * 180);
            attackDetector.RotateDetection(degree);
            var players = attackDetector.GetAllOfType<Player>();
            _attacking = false;
            if(!players.Any()) return;
            OnPlayerKill?.Invoke();
        }
        
        private void ChangeDirection()
        {
            var dir = Random.Next(0, 2);
            CurrentInput.basicInput = new Vector2(
               dir == 0 ? -1 : dir , CurrentInput.basicInput.y);
            _patrolTimer = Random.Next(2, 4);
        }
        
        private void EnterCombat(Character character)
        {
            if (!(character is Player player)) return;
            _combatMode = true;
            agroDetector.OnCharacterEnter -= EnterCombat;
            _player = player;
        }
    }
}