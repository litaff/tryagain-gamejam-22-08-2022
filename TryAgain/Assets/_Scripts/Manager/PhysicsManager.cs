using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public float Gravity { get; private set; }
    public float MaxJumpVelocity { get; private set; }
    public float MinJumpVelocity { get; private set; }

    public void Init(float maxJumpHeight, float minJumpHeight, float timeToJumpApex)
    {
        Gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        MaxJumpVelocity = Mathf.Abs(Gravity) * timeToJumpApex;
        MinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Gravity) * minJumpHeight);
    }
}
