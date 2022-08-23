using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using Unity.Mathematics;
using UnityEngine;

public class AfterimageManager : MonoBehaviour
{
    [SerializeField] private GameObject afterimagePrefab;
    private float _time;
    private List<AfterimageInput> _instructions;
    private List<Afterimage> _afterImages;
    private PhysicsManager _physics;
    private Vector2 _gameOrigin;

    public void Init(Vector2 gameOrigin, PhysicsManager physics)
    {
        _gameOrigin = gameOrigin;
        _physics = physics;
        _afterImages = new List<Afterimage>();
        _instructions = new List<AfterimageInput>();
        _time = 0;
        Player.OnCharacterInputChange += NewInstruction;
    }

    public Afterimage[] GetAfterimages()
    {
        return _afterImages.ToArray();
    }
    
    private void NewInstruction(CharacterInput input)
    {
        _instructions.Add(new AfterimageInput(input, _time)); 
        _time = 0;
    }
    // temp public
    public void NewAfterimage()
    {
        var image = 
            Instantiate(afterimagePrefab, _gameOrigin, quaternion.identity, transform);
        var afterimage = image.GetComponent<Afterimage>();
        
        afterimage.Init(_physics.Gravity, _physics.MaxJumpVelocity, _physics.MinJumpVelocity, _instructions.ToArray());
        _instructions = new List<AfterimageInput>();
        _afterImages.Add(afterimage);
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }
}
