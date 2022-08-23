using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PhysicsManager physicsManager;
    [SerializeField] private AfterimageManager afterimageManager;
    [SerializeField] private AfterimageController afterimageController;
    
    private void Awake()
    {
        physicsManager.Init(player.maxJumpHeight, player.minJumpHeight, player.timeToJumpApex);
        afterimageManager.Init(Vector2.zero ,physicsManager);
        player.Init(physicsManager.Gravity, physicsManager.MaxJumpVelocity, physicsManager.MinJumpVelocity);
        afterimageController.Init(afterimageManager.GetAfterimages());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            afterimageManager.NewAfterimage();
            afterimageController.Init(afterimageManager.GetAfterimages());
        }
    }
}
