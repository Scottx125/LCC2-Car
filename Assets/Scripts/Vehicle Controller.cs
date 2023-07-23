using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    // List of the wheels.
    [SerializeField]
    private List<Wheel> _wheelsList;
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private Transform _transform;

    [Header("Engine Power")]
    [SerializeField]
    private float _topSpeed;
    [SerializeField]
    private AnimationCurve _powerCurve;

    // List of wheels currently configured to allow steering
    // or power.
    private List<Wheel> _steerableWheels = new List<Wheel>();
    private List<Wheel> _driveWheels = new List<Wheel>();

    private float _forwardBackInput;

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_rb == null) _transform = GetComponent<Transform>();
        foreach(Wheel wheel in _wheelsList){
            wheel.Setup(_rb);
            UpdateWheel(wheel);
        }
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.W)){
            _forwardBackInput = Input.GetAxis("Vertical");
        }
        if (Input.GetKey(KeyCode.S)){
            _forwardBackInput = Input.GetAxis("Vertical");
        }
    }

    void FixedUpdate()
    {
        HandleInputForMovement();
    }

    private void HandleInputForMovement()
    {
        if (_forwardBackInput != 0.0f)
        {
            Acceleration();
        }
    }

    private void Acceleration()
    {
        Vector3 accelerationDirection = transform.forward;
        // Return the magnitude of the dot product between the cars forward direction and the 
        // cars current velocity.
        float carSpeed = Vector3.Dot(transform.forward, _rb.velocity);

        // Normalise the speed so we can use it to look up a value on the animation curve.
        float normSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / _topSpeed);

        // Torque
        float torquePercentage = _powerCurve.Evaluate(normSpeed) * _forwardBackInput;

        foreach (Wheel wheel in _driveWheels)
        {
            _rb.AddForceAtPosition(accelerationDirection * (torquePercentage * _topSpeed), wheel.transform.position);
        }
    }

    private void UpdateWheel(Wheel wheel){
        if (wheel.GetIsSteerable){
            _steerableWheels.Add(wheel);
        }
        if (wheel.GetIsPowered){
            _driveWheels.Add(wheel);
        }
    }
}
