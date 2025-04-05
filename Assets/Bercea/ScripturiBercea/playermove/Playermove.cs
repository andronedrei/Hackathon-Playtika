using UnityEngine;

// Enum pentru a alege tipul de control în Inspector
public enum ControlScheme { WASD, Arrows }

[RequireComponent(typeof(Rigidbody2D))] // Asigură prezența Rigidbody2D
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private ControlScheme controlType = ControlScheme.WASD; // Setează diferit în Inspector pentru fiecare player

    [Header("Components (Assign from Children)")]
    // Nu trebuie neapărat asignate manual dacă structura e corectă
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Referințe interne
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lastMoveDirection = Vector2.down; // Default Idle spre jos
    private bool isFrozen = false;

    // Stocare taste (va fi setat în Awake)
    private KeyCode keyUp, keyDown, keyLeft, keyRight;

    void Awake()
    {
        // Ia referința la Rigidbody de pe acest GameObject (Root)
        rb = GetComponent<Rigidbody2D>();

        // Încearcă să găsească Animator și SpriteRenderer pe copiii acestui GameObject
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null) Debug.LogError("Animator component not found on children!", this);
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer component not found on children!", this);

        // Setează tastele în funcție de tipul de control ales
        SetupKeys();
    }

    void Update()
    {
        // 1. Verifică starea de freeze
        if (isFrozen)
        {
            movementInput = Vector2.zero; // Oprește inputul dacă e înghețat
            // Oprește animația de mers dacă e necesar
            if (animator != null) animator.SetBool("IsMoving", false);
            return; // Nu procesa mai departe dacă e înghețat
        }

        // 2. Citește inputul
        ProcessPlayerInput();

        // 3. Actualizează animația
        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        // 4. Aplică mișcarea fizică (doar dacă nu e înghețat)
        if (!isFrozen && rb != null)
        {
            // Folosește MovePosition pentru mișcare stabilă care respectă coliziunile
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Funcție publică pentru a îngheța playerul din exterior
    public void Freeze()
    {
        isFrozen = true;
        movementInput = Vector2.zero; // Anulează inputul curent
        if (rb != null) rb.linearVelocity = Vector2.zero; // Oprește orice viteză reziduală
        if (animator != null) animator.SetBool("IsMoving", false); // Forțează animația de idle
        // Oprește orice altă acțiune specifică playerului aici (ex: tras)
        // Debug.Log(gameObject.name + " FROZEN");
    }

    // Funcție publică pentru a dezgheța playerul
    public void Unfreeze()
    {
        isFrozen = false;
        // Debug.Log(gameObject.name + " UNFROZEN");
    }


    // --- Metode Interne ---

    private void SetupKeys()
    {
        if (controlType == ControlScheme.WASD)
        {
            keyUp = KeyCode.W;
            keyDown = KeyCode.S;
            keyLeft = KeyCode.A;
            keyRight = KeyCode.D;
        }
        else // Arrows
        {
            keyUp = KeyCode.UpArrow;
            keyDown = KeyCode.DownArrow;
            keyLeft = KeyCode.LeftArrow;
            keyRight = KeyCode.RightArrow;
        }
    }

    private void ProcessPlayerInput()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(keyUp)) moveY = 1f;
        if (Input.GetKey(keyDown)) moveY = -1f;
        if (Input.GetKey(keyLeft)) moveX = -1f;
        if (Input.GetKey(keyRight)) moveX = 1f;

        // Normalizează vectorul pentru viteză constantă pe diagonală
        movementInput = new Vector2(moveX, moveY).normalized;
    }

    private void UpdateAnimationState()
    {
        if (animator == null) return; // Verificare siguranță

        bool isMoving = movementInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // Trimite direcția la animator
            animator.SetFloat("MoveX", movementInput.x);
            animator.SetFloat("MoveY", movementInput.y);
            lastMoveDirection = movementInput; // Salvează ultima direcție validă

            // Flip Sprite Renderer pentru stânga/dreapta
            if (spriteRenderer != null)
            {
                if (movementInput.x < -0.1f) spriteRenderer.flipX = true;
                else if (movementInput.x > 0.1f) spriteRenderer.flipX = false;
                // Nu schimba flip dacă mișcarea e strict verticală
            }
        }
        else // Stând pe loc (Idle)
        {
            // Setează animatorul la ultima direcție pentru idle corect
            animator.SetFloat("MoveX", lastMoveDirection.x);
            animator.SetFloat("MoveY", lastMoveDirection.y);
        }
    }
}