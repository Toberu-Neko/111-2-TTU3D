using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroManager : MonoBehaviour
{
    public static GyroManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private Gyroscope gyro;
    private Quaternion rotation;
    private bool gyroActive;

    public void EnableGyro()
    {
        if (gyroActive)
            return;

        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroActive = gyro.enabled;
        }
        else
            Debug.Log("Gyro not supported.");

    }
    private void Start()
    {
        EnableGyro();
    }
    private void Update()
    {
        if (gyroActive)
        {
            rotation = gyro.attitude;
            //Debug.Log(rotation);
        }
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }
}
