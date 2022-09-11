using System;
using System.Collections.Generic;
using _Scripts.Manager;
using UnityEditor;
using UnityEngine;

namespace _Scripts
{
    [ExecuteInEditMode]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private List<Spawner> spawners;
        [SerializeField] private LockedDoor[] doors;
        [SerializeField] private Key key;
        /// <summary>
        /// This should be unique to all tiles, there can't be two the same
        /// </summary>
        private int _id;

        private Vector2 _offset;

        private static int _tileCount;

        public void Init(NpcManager npcManager, Vector2 offset)
        {
            _offset = offset;
            _tileCount++;
            _id = _tileCount;
            foreach (var door in doors)
            {
                door.Init(_id);
            }
            key.Init(_id);
            foreach (var spawner in spawners)
            {
                npcManager.NewNpc(spawner.npc,spawner.position + offset);
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var spawner in spawners)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(spawner.position+_offset,1f);
            }
        }

        [Serializable]
        public struct Spawner
        {
            public Npc npc;
            public Vector2 position;
        }
    }
}