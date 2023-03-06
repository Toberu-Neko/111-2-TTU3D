using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class IntersectionOld : MonoBehaviour
{
    private List<Vector2> groundPosition = new List<Vector2>();
    private List<float> groundRotationY = new List<float>();
    private float playerGroundRotationY;
    private Vector2 playerGroundPos;
    
    
    void Start()
    {
        RaycastHit[] hit = Physics.BoxCastAll(transform.position, transform.localScale / 2, Vector3.down, Quaternion.identity, 1f, LayerMask.GetMask("Ground"));
        foreach (RaycastHit ray in hit)
        {
            groundPosition.Add(new Vector2(ray.transform.position.x, ray.transform.position.z));
            groundRotationY.Add(ray.transform.eulerAngles.y);
            Debug.Log(new Vector2(ray.transform.position.x, ray.transform.position.z));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 playerDir = new Vector2 (other.gameObject.transform.position.x, other.gameObject.transform.position.z) - new Vector2 (transform.position.x, transform.position.z);
            
            foreach (Vector2 pos in groundPosition)
            {
                Vector2 t_groundDir = pos - new Vector2(transform.position.x, transform.position.z);
                Debug.Log(pos + " ªº¨¤«× " + Vector2.SignedAngle(playerDir, t_groundDir));
            }


        }
    }
    void Update()
    {
        
    }
}
