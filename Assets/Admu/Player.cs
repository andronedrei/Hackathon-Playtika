using UnityEngine;

public enum PoolThiefType {
    Type_1,
    Type_2
}

public enum ControlType {
    Arrows,
    WASD
}

public class Player : MovableEntity
{
    [SerializeField] protected PoolThiefType pool_thief_type;
    [SerializeField] protected ControlType control_type;
    
    protected KeyCode[] movement_keys = new KeyCode[4];

    protected override void HandleMovement()
    {
        Vector2 movement = Vector2.zero;

        bool up = Input.GetKey(movement_keys[0]);
        bool down = Input.GetKey(movement_keys[1]);
        bool left = Input.GetKey(movement_keys[2]);
        bool right = Input.GetKey(movement_keys[3]);

        if (up)
            movement.y += 1.0f;
        if (down)
            movement.y -= 1.0f;
        if (right)
            movement.x += 1.0f;
        if (left)
            movement.x -= 1.0f;

        // pt viteza consistenta
        movement = movement.normalized;

        // Apply the movement
        //rb.linearVelocity = movement * speed * Time.deltaTime;
        transform.Translate(movement * speed * Time.deltaTime);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = base_speed;

        switch (pool_thief_type) {
            case PoolThiefType.Type_1:
                TimeManager.Instance.SubscribeThief_1(this);
                break;
            case PoolThiefType.Type_2:
                TimeManager.Instance.SubscribeThief_2(this);
                break;
            default:
                break;
        }

        switch (control_type)
        {
            case ControlType.WASD:
                movement_keys[0] = KeyCode.W; // Up
                movement_keys[1] = KeyCode.S; // Down
                movement_keys[2] = KeyCode.A; // Left
                movement_keys[3] = KeyCode.D; // Right
                break;

            default:
                movement_keys[0] = KeyCode.UpArrow;
                movement_keys[1] = KeyCode.DownArrow;
                movement_keys[2] = KeyCode.LeftArrow;
                movement_keys[3] = KeyCode.RightArrow;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
}
