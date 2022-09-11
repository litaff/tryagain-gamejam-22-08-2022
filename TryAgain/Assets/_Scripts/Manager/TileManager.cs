using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace _Scripts.Manager
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private Tile tutorialTile;
        [SerializeField] private List<Tile> tiles;
        private List<Tile> _activeTiles;
        private NpcManager _npcManager;
        private Random _random;
        private Vector2 _nextTilePosition;

        private void Start()
        {
            Key.OnPickUp += GenerateTile;
        }

        public void Init(NpcManager npcManager, Random random)
        {
            DeInit();
            _npcManager = npcManager;
            _random = random;
            _activeTiles = new List<Tile>();
            _nextTilePosition = new Vector2(0, 0);
            GenerateThisTile(tutorialTile);
        }

        private void DeInit()
        {
            if(_activeTiles is null) return;
            foreach (var activeTile in _activeTiles.ToArray())
            {
                Destroy(activeTile.gameObject);
            }
        }
        
        private void GenerateTile()
        {
            var tileNr = _random.Next(0, tiles.Count);
            GenerateThisTile(tiles[tileNr]);
        }

        private void GenerateThisTile(Tile tileToGenerate)
        {
            var tile = Instantiate(
                tileToGenerate, _nextTilePosition, quaternion.identity, transform);
            tile.Init(_npcManager, _nextTilePosition);
            _activeTiles.Add(tile);
            _nextTilePosition += new Vector2(64f, 0f);
        }
        
        
    }
}