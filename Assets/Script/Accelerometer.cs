using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometer : MonoBehaviour
{
    void Update()
    {
        Debug.Log(Input.acceleration);
    }
}
