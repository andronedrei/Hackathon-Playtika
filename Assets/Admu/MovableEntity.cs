using UnityEngine;

public abstract class MovableEntity : MonoBehaviour, IFreezable
{
    [SerializeField] protected float base_speed;
    [SerializeField] protected Rigidbody2D rb;
    protected float speed;
    protected Vector2 movement_dir;

    protected abstract void HandleMovement();

    protected void TranslateEntity() {
        transform.Translate(speed * Time.deltaTime * movement_dir);
    }

    void IFreezable.OnFreeze()
    {
        speed = 0;
    }

    void IFreezable.OnUnfreeze()
    {
        speed = base_speed;
    }
}
