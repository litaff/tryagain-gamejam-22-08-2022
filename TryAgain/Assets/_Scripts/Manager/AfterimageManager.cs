using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using _Scripts.Manager;
using Unity.Mathematics;
using UnityEngine;

public class AfterimageManager : MonoBehaviour
{
    [SerializeField] private Afterimage afterimagePrefab;
    [SerializeField] private AfterimageController afterimageController;
    private List<AfterimageInput> _instructions;
    private List<List<AfterimageInput>> _afterimages;
    private List<Afterimage> _activeAfterimages;
    private PhysicsManager _physics;
    private Vector2 _gameOrigin;


    private void Start()
    {
        Player.OnCharacterInputChange += NewInstruction;
    }

    public void Init(Vector2 gameOrigin, PhysicsManager physics)
    {
        DeInit();
        _gameOrigin = gameOrigin;
        _physics = physics;
        _activeAfterimages = new List<Afterimage>();
        _afterimages ??= new List<List<AfterimageInput>>();
        if (_instructions is null)
            _instructions = new List<AfterimageInput>();
        else
            NewAfterimageInput();

        foreach (var afterimage in _afterimages)
        {
            NewAfterimage(afterimage);
        }
        TimeManager.TimePassedFor(this);
        afterimageController.Init(_activeAfterimages.ToArray());
    }

    private void DeInit()
    {
        if(_activeAfterimages is null) return;
        foreach (var activeAfterimage in _activeAfterimages.ToArray())
        {
            Destroy(activeAfterimage.gameObject);
        }
    }
    
    private void NewInstruction(CharacterInput input)
    {
        _instructions.Add(new AfterimageInput(input, TimeManager.TimePassedFor(this)));
    }

    private void NewAfterimageInput()
    {
        NewInstruction(new CharacterInput()); // make sure the last input is 0
        _afterimages.Add(_instructions);
        _instructions = new List<AfterimageInput>();
    }
    
    private void NewAfterimage(List<AfterimageInput> instructions)
    {
        var image = 
            Instantiate(afterimagePrefab.gameObject, _gameOrigin, quaternion.identity, transform);
        var afterimage = image.GetComponent<Afterimage>();

        afterimage.Init(_physics.Gravity, _physics.MaxJumpVelocity, _physics.MinJumpVelocity, instructions.ToArray());
        _instructions = new List<AfterimageInput>();
        _activeAfterimages.Add(afterimage);
        
        afterimageController.Init(_activeAfterimages.ToArray());
    }
}
