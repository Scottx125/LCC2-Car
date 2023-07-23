using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool turnLeft;
    public bool turnRight;
    public bool forward;
    public bool backward;

    [Header("Car Control")]
    [SerializeField]
    private bool _isSteerable;
    [SerializeField]
    private bool _isPowered;

    [Header("Suspension")]
    [SerializeField]
    private float _suspensionLength;
    [SerializeField]
    private float _suspensionStrength;
    [SerializeField]
    private float _suspensionDamperStrength;

    private Rigidbody _parentRB;
    private RaycastHit _wheelRayHit;
    private bool _wheelRaycastBool;

    public bool GetIsSteerable => _isSteerable;
    public bool GetIsPowered => _isPowered;
    
    public void Setup(Rigidbody rb)
    {
        _parentRB = rb;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Cast a ray to act as the wheel touching the ground (for suspension etc).
        // We'll check this before doing any calculations.
        _wheelRaycastBool = Physics.Raycast(transform.position, -transform.up, out _wheelRayHit, _suspensionLength);
        
        CalculateSuspension();
    }

    private void CalculateSuspension()
    {
        
    }
}
