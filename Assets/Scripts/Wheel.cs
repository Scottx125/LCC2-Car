using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField]
    private bool _isSteerable;
    [SerializeField]
    private bool _isPowered;

    public bool GetIsSteerable => _isSteerable;
    public bool GetIsPowered => _isPowered;
    // Just incase it's needed...
    public void Setup()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
