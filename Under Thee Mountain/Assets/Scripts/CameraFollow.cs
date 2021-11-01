using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject PlayerReference;
    private const float _offset = 10f;

    private void Update()
    {
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        if(transform.position != new Vector3(PlayerReference.transform.position.x, 
            PlayerReference.transform.position.y + _offset, 
            PlayerReference.transform.position.z))
        {
            transform.position = new Vector3(
                PlayerReference.transform.position.x, 
                PlayerReference.transform.position.y + _offset, 
                PlayerReference.transform.position.z
            );
        }
    }
}
