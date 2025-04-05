using UnityEngine;

public abstract class MovableEntity : MonoBehaviour, Freezable
{
    [SerializeField] protected float base_speed;
    [SerializeField] protected Rigidbody2D rb;
    protected float speed;
    protected Vector2 movement_dir;

    protected abstract void HandleMovement();

    protected void TranslateEntity() {
        transform.Translate(speed * Time.deltaTime * movement_dir);
    }

    void Freezable.OnFreeze()
    {
        speed = 0;
    }

    void Freezable.OnUnfreeze()
    {
        speed = base_speed;
    }
}
