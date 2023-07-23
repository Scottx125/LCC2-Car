using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
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

    [Header("Tyre")]
    [SerializeField]
    private Transform _tyreObject;
    [SerializeField]
    private float _minVisualSuspension;
    [SerializeField]
    private float _tyreRadius;
    [SerializeField]
    private AnimationCurve _tyreAnimationCurve;
    [SerializeField]
    private float _tyreMass;
    [SerializeField]
    private float _turnRate;
    [SerializeField]
    private float _maxTurnAngle;

    private Rigidbody _parentRB;
    private Transform _parentTransform;
    private RaycastHit _wheelRayHit;
    private bool _wheelRaycastBool;
    private float _leftRightInput;

    public void SetLeftRightInput(float x) => _leftRightInput = x;
    public bool GetIsSteerable => _isSteerable;
    public bool GetIsPowered => _isPowered;
    
    public void Setup(Rigidbody rb)
    {
        _parentRB = rb;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 localDown = Vector3.down;
        Vector3 worldDown = transform.TransformDirection(localDown);
        // Cast a ray to act as the wheel touching the ground (for suspension etc).
        // We'll check this before doing any calculations.
        _wheelRaycastBool = Physics.Raycast(transform.position, worldDown, out _wheelRayHit, _suspensionLength);
        CalculateSuspension();
        TurnWheel();
        CalculateSliding();
    }

    private void CalculateMeshPosition(float suspensionOffset)
    {
        if (_tyreObject == null) return;
        _tyreObject.transform.position = new Vector3(transform.position.x, 
        Mathf.Clamp(transform.position.y - (_suspensionLength - _wheelRayHit.distance) - _tyreRadius, _suspensionLength - _tyreRadius, transform.position.y - _minVisualSuspension)
        , transform.position.z);
    }

    private void TurnWheel()
    {
        float amountToRotateThisFrame = _turnRate * Time.deltaTime;

        // Calculate the target rotation based on the clamped angle
        Quaternion targetRotation = Quaternion.Euler(0f, _maxTurnAngle * _leftRightInput, 0f);

        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, amountToRotateThisFrame);
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
            CalculateMeshPosition(suspensionOffset);
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

    private void CalculateSliding()
    {
        if (_wheelRaycastBool){
            // World space direction of sliding force if grip is lost.
            Vector3 slipDir = transform.right;

            // World space velocity of the wheel.
            Vector3 tireWorldVel = _parentRB.GetPointVelocity(transform.position);

            // Velocity result of the direction the wheel is turning in + the current velocity direction.
            float steeringVel = Vector3.Dot(slipDir, tireWorldVel);

            // Change velocity against the slide by the opposite direction * gripfactor.
            float desiredVelChange = -steeringVel * CalculateGripFactor(steeringVel, tireWorldVel);

            // Change desired velocity into acceleration. Acceleration is change in velocity / time.
            float desiredAcceleration = desiredVelChange / Time.fixedDeltaTime;

            _parentRB.AddForceAtPosition(slipDir * _tyreMass * desiredAcceleration, transform.position);
        }
    }
        
    private float CalculateGripFactor(float steeringVel, Vector3 tireWorldVel)
    {
        // Gets the evaluated Y axis value of the animaiton curve for % traction
        // via the X axis of % of tyre velocity towards transform.right.
        return _tyreAnimationCurve.Evaluate(steeringVel / tireWorldVel.magnitude);
    }
}
