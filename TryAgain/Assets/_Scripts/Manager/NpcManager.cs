using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace _Scripts.Manager
{
    public class NpcManager : MonoBehaviour
    {
        private List<Npc> _activeNpcs;
        private PhysicsManager _physics;
        private Random _random;

        public void Init(PhysicsManager physics, Random random)
        {
            DeInit();
            _physics = physics;
            _random = random;
            _activeNpcs = new List<Npc>();
        }

        public void NewNpc(Npc npc, Vector2 position)
        {
            var gObject = Instantiate(npc, position, quaternion.identity, transform);
            gObject.Init(_physics.Gravity, _physics.MaxJumpVelocity, _physics.MinJumpVelocity, _random);
            _activeNpcs.Add(gObject);
        }

        private void KillNpc(Npc npc)
        {
            _activeNpcs.Remove(npc);
            npc.Die();
        }

        private void DeInit()
        {
            if(_activeNpcs is null) return;
            foreach (var activeNpc in _activeNpcs.ToArray())
            {
                Destroy(activeNpc.gameObject);
            }
        }

        private void Start()
        {
            Player.OnNpcKill += KillNpc;
            Afterimage.OnNpcKill += KillNpc;
        }

        private void Update()
        {
            foreach (var npc in _activeNpcs)
            {
                npc.Act();
            }
        }
    }
}