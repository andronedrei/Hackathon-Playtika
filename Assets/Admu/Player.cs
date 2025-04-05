using UnityEngine;

// Enum-urile raman la fel
public enum PoolThiefType { Type_1, Type_2 }
public enum ControlType { Arrows, WASD }

public class Player : MovableEntity
{
    [SerializeField] protected PoolThiefType pool_thief_type;
    [SerializeField] protected ControlType control_type;

    protected KeyCode[] movement_keys = new KeyCode[4];

    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Vector2 lastMovementDirection = Vector2.down; // Adaugam pentru idle corect

    private void Awake() // Este mai bine sa folosesti Awake pentru GetComponent
    {
        // Incearca sa gasesti componentele si in copii daca structura ta e GFX/Collision
        myAnimator = GetComponentInChildren<Animator>();
        mySpriteRender = GetComponentInChildren<SpriteRenderer>();

        // Verificari pentru a te asigura ca au fost gasite
        if (myAnimator == null) Debug.LogError("Animator component not found!", this);
        if (mySpriteRender == null) Debug.LogError("SpriteRenderer component not found!", this);
    }

    protected override void HandleMovement()
    {
        movement_dir = Vector2.zero; // Resetam directia

        bool up = Input.GetKey(movement_keys[0]);
        bool down = Input.GetKey(movement_keys[1]);
        bool left = Input.GetKey(movement_keys[2]);
        bool right = Input.GetKey(movement_keys[3]);

        // Logica pentru sus/jos ramane la fel
        if (up)
            movement_dir.y += 1.0f;
        if (down)
            movement_dir.y -= 1.0f;

        // --- COREctIE AICI ---
        if (right)
        { // Adauga acolada de deschidere
            mySpriteRender.flipX = false;
            movement_dir.x += 1.0f;
        } // Adauga acolada de inchidere

        if (left)
        { // Adauga acolada de deschidere
            mySpriteRender.flipX = true;
            movement_dir.x -= 1.0f;
        } // Adauga acolada de inchidere
        // --- Sfarsit COREctIE ---

        // Normalizam pentru viteza consistenta
        movement_dir = movement_dir.normalized;

        // Aplicam miscarea (din MovableEntity, probabil transform.Translate)
        TranslateEntity();

        // Actualizam animatorul
        UpdateAnimator();
    }

    // Metoda separata pentru actualizarea animatorului
    private void UpdateAnimator()
    {
        if (myAnimator == null) return; // Verificare siguranta

        bool isMoving = movement_dir.sqrMagnitude > 0.01f;
        myAnimator.SetBool("IsMoving", isMoving); // Adauga si parametrul IsMoving in Animator
        Debug.Log($"Frame: {Time.frameCount} | isMoving = {isMoving} | Current movement_dir: {movement_dir}");
        if (isMoving)
        {
            myAnimator.SetFloat("MoveX", movement_dir.x);
            myAnimator.SetFloat("MoveY", movement_dir.y);
            lastMovementDirection = movement_dir; // Salveaza ultima directie valida
        }
        else // Idle
        {
            // Foloseste ultima directie pentru idle corect
            myAnimator.SetFloat("MoveX", lastMovementDirection.x);
            myAnimator.SetFloat("MoveY", lastMovementDirection.y);
        }
    }


    void Start()
    {
        // Logica din Start ramane la fel pentru setarea tastelor si subscribe
        // movement_dir = Vector2.zero; // Deja setat in Awake/HandleMovement
        speed = base_speed; // Asigura-te ca speed si base_speed sunt initializate corect in MovableEntity

        SubscribeToTimeManager(); // Muta subscribe aici daca TimeManager e gata doar in Start
        SetupKeys(); // Seteaza tastele
    }

    // SetupKeys si SubscribeToTimeManager (separate pentru claritate)
    private void SetupKeys()
    {
        switch (control_type)
        {
            case ControlType.WASD:
                movement_keys[0] = KeyCode.W; movement_keys[1] = KeyCode.S;
                movement_keys[2] = KeyCode.A; movement_keys[3] = KeyCode.D;
                break;
            default: // Arrows ca default
                movement_keys[0] = KeyCode.UpArrow; movement_keys[1] = KeyCode.DownArrow;
                movement_keys[2] = KeyCode.LeftArrow; movement_keys[3] = KeyCode.RightArrow;
                break;
        }
    }

    private void SubscribeToTimeManager()
    {
        if (TimeManager.Instance != null)
        {
            switch (pool_thief_type)
            {
                case PoolThiefType.Type_1: TimeManager.Instance.SubscribeThief_1(this); break;
                case PoolThiefType.Type_2: TimeManager.Instance.SubscribeThief_2(this); break;
            }
        }
        else
        {
            Debug.LogWarning("TimeManager Instance not found when Player tried to subscribe.", this);
        }
    }

    // folosim fixed update pentru consistenta cu atributele de fizica
    // ATENTIE: Daca MovableEntity.TranslateEntity NU foloseste Rigidbody,
    // chemarea din FixedUpdate poate cauza jitter. Daca MovableEntity foloseste
    // transform.Translate, e mai bine sa chemi HandleMovement din Update().
    // Daca MovableEntity foloseste Rigidbody.MovePosition, FixedUpdate e corect.
    // Presupunand ca MovableEntity foloseste transform.Translate, mutam in Update:
    /* void FixedUpdate()
    {
       // HandleMovement(); // Comentat sau sters
    } */

    void Update() // Cheama HandleMovement din Update daca folosesti transform.Translate
    {
        // Adauga verificarea pentru freeze si aici daca nu o face MovableEntity
        // if (isFrozen) { /* logica de freeze */ return; }

        HandleMovement();
    }


} // Sfarsitul clasei Player