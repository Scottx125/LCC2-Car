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

    [Header("Engine Power")]
    [SerializeField]
    private float _topSpeed;
    [SerializeField]
    private AnimationCurve _powerCurve;

    // List of wheels currently configured to allow steering
    // or power.
    private List<Wheel> _steerableWheels = new List<Wheel>();
    private List<Wheel> _driveWheels = new List<Wheel>();

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        foreach(Wheel wheel in _wheelsList){
            wheel.Setup(_rb);
            UpdateWheel(wheel);
        }
    }

    void FixedUpdate()
    {
        HandleInputForMovement();
    }

    private void HandleInputForMovement()
    {
        float xMovement = Input.GetAxis("Horizontal");   
        float zMovement = Input.GetAxis("Vertical");
        if (zMovement != 0.0f)
        {
            Acceleration(zMovement);
        }
        if (xMovement != 0.0f){
            foreach (Wheel wheel in _steerableWheels){
                wheel.SetLeftRightInput(xMovement);
            }
        }
    }

    private void Acceleration(float zMovement)
    {
        Vector3 accelerationDirection = transform.forward;
        // Return the magnitude of the dot product between the cars forward direction and the 
        // cars current velocity.
        float carSpeed = Vector3.Dot(transform.forward, _rb.velocity);

        // Normalise the speed so we can use it to look up a value on the animation curve.
        float normSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / _topSpeed);

        // Torque
        float torquePercentage = _powerCurve.Evaluate(normSpeed) * zMovement;

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
