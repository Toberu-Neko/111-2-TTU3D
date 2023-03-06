using System.Linq;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    [SerializeField] private Transform[] interSections;
    [SerializeField] private int minAngle;
    private Vector2[] interSectionsDir;

    private float turnRightAngle;
    private float turnLeftAngle;
    private float turnStraightAngle;
    private bool incline;


    private void Start()
    {
        if (interSections.Length <= 0) 
            gameObject.SetActive(false);

        ResetVariables();

        incline = true;
        interSectionsDir = new Vector2[interSections.Length];

        for(int i = 0; i < interSections.Length; i++)
        {
            interSectionsDir[i] = new Vector2((transform.position - interSections[i].position).normalized.x, (transform.position - interSections[i].position).normalized.z);
        }
    }
    private void ResetVariables()
    {
        turnRightAngle = 90;
        turnLeftAngle = 90;
        turnStraightAngle = 180;
        incline = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PlayerStatus.inIntersection && incline)
                other.gameObject.GetComponent<PlayerMovement>().goIncline = true;

            ResetVariables();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStatus.inIntersection = true;

            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            Vector2 playerDir = new(other.gameObject.transform.forward.x, other.gameObject.transform.forward.z);
            float[] pathAngle = new float[interSections.Length];

            for (int i = 0; i < interSectionsDir.Length; i++)
            {
                pathAngle[i] = Vector2.SignedAngle(playerDir, interSectionsDir[i]);

                if(pathAngle[i] > 180 - minAngle || pathAngle[i] < -180 + minAngle)
                {
                    incline = false;
                }
            }
            if (incline)
            {
                if (Mathf.Abs(pathAngle.Max()) > Mathf.Abs(pathAngle.Min()))
                    turnStraightAngle = pathAngle.Max();
                else
                    turnStraightAngle = pathAngle.Min();

                Debug.Log("turnStraightAngle = " + turnStraightAngle);
            }
            for (int i = 0; i < pathAngle.Length; i++)
            {
                if(pathAngle[i] != turnStraightAngle)
                {
                    // - Left + Right
                    if(pathAngle[i] > minAngle && pathAngle[i] < 180- minAngle)
                    {
                        turnRightAngle = pathAngle[i];
                        Debug.Log("turnRightAngle = " + turnRightAngle);
                    }
                    if(pathAngle[i] < -minAngle && pathAngle[i] > -180 + minAngle)
                    {
                        turnLeftAngle = pathAngle[i];
                        Debug.Log("turnLeftAngle = " + turnLeftAngle);
                    }
                }
            }
            playerMovement.turnRightAngle = 180-turnRightAngle;
            playerMovement.turnLeftAngle = 180-turnLeftAngle;
            playerMovement.turnStraightAngle = 180 - turnStraightAngle;
        }
    }
}
