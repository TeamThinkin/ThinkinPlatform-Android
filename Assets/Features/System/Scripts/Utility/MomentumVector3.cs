using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MomentumVector3
{
    public Vector3 Value;
    public float DecayRate;

    private Vector3 smoothValue;

    public MomentumVector3(float DecayRate)
    {
        this.Value = Vector3.zero;
        this.smoothValue = this.Value;
        this.DecayRate = DecayRate;
    }

    public void Set(Vector3 Value)
    {
        this.Value = Value;
        smoothValue = Vector3.Lerp(smoothValue, Value, 0.1f);
    }

    public void Update()
    {
        smoothValue = Value = Vector3.Lerp(smoothValue, Vector3.zero, DecayRate);
    }
}