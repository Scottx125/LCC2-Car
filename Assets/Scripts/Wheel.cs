using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    protected float turnLeft;
    protected float turnRight;

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
    private Transform _parentTransform;
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
        if (_wheelRaycastBool)
        {
            // World space direction of the suspension force (we are applying
            // force to the parent rigidbody at the location we are at in world space).
            Vector3 suspensionDir = transform.up;

            // World space velocity of the wheel (we need the current velocity of that position in world space).
            Vector3 tireWorldVel = _parentRB.GetPointVelocity(transform.position);

            // Offset from the resting distance of the suspension based on the distance from the ground.
            float suspensionOffset = _suspensionLength - _wheelRayHit.distance;

            // Velocity along the suspension direction.
            // Get the dot product (magnitude) of the upwards direction the suspension wants to move in
            // against the current velocity at the suspension point.
            float vel = Vector3.Dot(suspensionDir, tireWorldVel);

            // Calculate the total suspension force strength - the dampened velocity.
            float force = (suspensionOffset * _suspensionStrength) - (vel * _suspensionDamperStrength);

            // Apply the force at the position of the wheel in the direction of the suspension.
            _parentRB.AddForceAtPosition(suspensionDir * force, transform.position);
        }
    }

}
