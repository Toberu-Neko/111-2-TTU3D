using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGyro : MonoBehaviour
{
    [SerializeField]
    Quaternion baseRotateion = new(0,0,1,0);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(GyroManager.instance.GetRotation());
        transform.localRotation = GyroManager.instance.GetRotation() * baseRotateion;
    }
}
