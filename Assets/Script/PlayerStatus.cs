using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static bool moveable;
    public static bool inIntersection;
    public static bool turned;
    // Start is called before the first frame update
    void Start()
    {
        moveable = true;
        inIntersection = false;

    }

    void Update()
    {
        
    }

}
