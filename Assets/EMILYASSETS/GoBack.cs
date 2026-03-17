using UnityEngine;

public class GoBack : MonoBehaviour
{
    
    public Transform startPoint; // assign level start here

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoBack"))
        {
            other.transform.position = startPoint.position; 
        }
    }
    }

