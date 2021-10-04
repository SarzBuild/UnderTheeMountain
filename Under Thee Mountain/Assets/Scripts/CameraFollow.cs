using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject PlayerReference;
    public float Offset = 10f;

    private void Update()
    {
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        if((transform.position != new Vector3(PlayerReference.transform.position.x, 
            PlayerReference.transform.position.y + Offset, 
            PlayerReference.transform.position.z)))
        {
            transform.position = new Vector3(
                PlayerReference.transform.position.x, 
                PlayerReference.transform.position.y + Offset, 
                PlayerReference.transform.position.z
            );
        }
    }
}
