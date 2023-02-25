using UnityEngine;
using UnityEngine.InputSystem;

public class AccelerometerManager : MonoBehaviour
{
    public static AccelerometerManager instance;
    private void Awake()
    {
        if(instance != null)
            return;
        else
            instance = this;
    }
    private Vector3 acceleration;

    private void Update()
    {
        acceleration = Accelerometer.current.acceleration.ReadValue();
        /* if (SystemInfo.supportsAccelerometer)
        {
            acceleration = Input.acceleration;
        }
        else
        {
            Debug.Log("Your phone don't support accelerometer.");
        } */
    }
    public Vector3 GetAcceleration()
    {
        return acceleration;
    }
}
