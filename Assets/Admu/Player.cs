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
    // pool pt freeze time. (DACA VREI SA ISI IA JUCATORII FREEZE IN ACELASI TIMP II BAGI IN ACELASI POOL)
    [SerializeField] protected PoolThiefType pool_thief_type;
    // tip de control jucator (sageti sau "WASD")
    [SerializeField] protected ControlType control_type;
    
    protected KeyCode[] movement_keys = new KeyCode[4];

    protected override void HandleMovement()
    {
        movement_dir = Vector2.zero;

        bool up = Input.GetKey(movement_keys[0]);
        bool down = Input.GetKey(movement_keys[1]);
        bool left = Input.GetKey(movement_keys[2]);
        bool right = Input.GetKey(movement_keys[3]);

        if (up)
            movement_dir.y += 1.0f;
        if (down)
            movement_dir.y -= 1.0f;
        if (right)
            movement_dir.x += 1.0f;
        if (left)
            movement_dir.x -= 1.0f;

        // pt viteza consistenta
        movement_dir = movement_dir.normalized;

        // aplicam miscarea de translatie pentru deplasare efectiva
        TranslateEntity(); 
    }

    void Start()
    {
        movement_dir = Vector2.zero;
        speed = base_speed;

        // "suncribe" la pool-ul corespunzator cammpului ales in "Inspector"
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

        // setam tastele
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

    void Update()
    {
        HandleMovement();
    }
}
