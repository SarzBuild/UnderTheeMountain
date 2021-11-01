using System.Collections;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    protected IEnumerator DestroyAfterTimeAmount(float seconds,GameObject gameObjectToDestroy)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObjectToDestroy);
    }
}
