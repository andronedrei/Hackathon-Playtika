using UnityEngine;

public abstract class MovableEntity : MonoBehaviour, Freezable
{
    [SerializeField] protected float base_speed;
    [SerializeField] protected Rigidbody2D rb;
    protected float speed;

    protected abstract void HandleMovement();

    void Freezable.OnFreeze()
    {
        speed = 0;
    }

    void Freezable.OnUnfreeze()
    {
        speed = base_speed;
    }
}
