using UnityEngine;

public class GenericMove : MonoBehaviour
{
    public TransformTypes UsedTransfrom;
    public enum TransformTypes
    {
        FORWARDS,
        BACKWARDS,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    
    public float Speed;
    private Vector3 _direction;

    void Update()
    {
        _direction = UsedTransfrom switch
        {
            TransformTypes.UP => transform.up,
            TransformTypes.DOWN => -transform.up,
            TransformTypes.RIGHT => transform.right,
            TransformTypes.LEFT => -transform.right,
            TransformTypes.FORWARDS => transform.forward,
            TransformTypes.BACKWARDS => -transform.forward,
            _ => _direction
        };
        transform.position += _direction * Speed;
    }
}
