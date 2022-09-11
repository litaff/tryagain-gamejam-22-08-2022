using System;
using _Scripts;
using _Scripts.Manager;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2 initialPlayerPosition;
    [SerializeField] private Player player;
    [SerializeField] private PhysicsManager physicsManager;
    [SerializeField] private AfterimageManager afterimageManager;
    [SerializeField] private NpcManager npcManager;
    [SerializeField] private TileManager tileManager;
    private Random _random;
    private int _seed;
    
    private void Awake()
    {
        New();
    }

    /// <summary>
    /// Call this when a game is to be created
    /// </summary>
    private void New()
    {
        _seed = Guid.NewGuid().GetHashCode(); // https://stackoverflow.com/questions/1785744/how-do-i-seed-a-random-class-to-avoid-getting-duplicate-random-values
        _random = new Random(_seed);
        physicsManager.Init(player.maxJumpHeight, player.minJumpHeight, player.timeToJumpApex);
        afterimageManager.Init(initialPlayerPosition ,physicsManager);
        npcManager.Init(physicsManager, _random);
        tileManager.Init(npcManager,_random);
        player.Init(physicsManager.Gravity, physicsManager.MaxJumpVelocity, physicsManager.MinJumpVelocity, initialPlayerPosition);
    }
    
    /// <summary>
    /// Call this when a game is to be continued
    /// </summary>
    private void Continue()
    {
        _random = new Random(_seed);
        afterimageManager.Init(initialPlayerPosition ,physicsManager);
        npcManager.Init(physicsManager, _random);
        tileManager.Init(npcManager,_random);
        player.Init(physicsManager.Gravity, physicsManager.MaxJumpVelocity, physicsManager.MinJumpVelocity, initialPlayerPosition);
    }

    private void Update()
    {
        if (player.Dead)
        {
            Continue();
        }
        TimeManager.PassTime(Time.deltaTime);
    }
}
