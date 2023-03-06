using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    private List<Vector3> groundPosition = new List<Vector3>();
    private float[] groundRotationY;
    
    void Start()
    {
        RaycastHit[] hit = Physics.BoxCastAll(transform.position, transform.localScale / 2, Vector3.down, Quaternion.identity, 0f, LayerMask.GetMask("Ground"));

        groundRotationY = new float[hit.Length];
        int count = 0;
        foreach (RaycastHit ray in hit)
        {
            groundRotationY[count] = ray.collider.gameObject.transform.eulerAngles.y;
            Debug.Log("Ground: " + ray.collider.gameObject.name);
            count++;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float playerRotationY = other.gameObject.GetComponent<PlayerMovement>().GetGroundRotation();

            Debug.Log("Player ground: " + playerRotationY);
            for (int i = 0; i < groundRotationY.Length; i++) 
            {
                Debug.Log(playerRotationY - groundRotationY[i]);
            }

        }
    }

    void Update()
    {
        
    }
}
