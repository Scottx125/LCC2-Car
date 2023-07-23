using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    // List of the wheels.
    [SerializeField]
    private List<Wheel> _wheelsList;

    // List of wheels currently configured to allow steering
    // or power.
    private List<Wheel> _steerableWheels = new List<Wheel>();
    private List<Wheel> _driveWheels = new List<Wheel>();

    private void Awake()
    {
        foreach(Wheel wheel in _wheelsList){
            wheel.Setup();
            UpdateWheel(wheel);
        }
    }
    
    private void Update()
    {
        
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
