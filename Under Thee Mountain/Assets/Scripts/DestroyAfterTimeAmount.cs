public class DestroyAfterTimeAmount : SelfDestroy
{
    public float Seconds;
    
    private void OnEnable()
    {
        if (transform.parent != null) transform.parent = null;
        StartCoroutine(DestroyAfterTimeAmount(Seconds,gameObject));
    }
}
