using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))] // Recomandat
public class Cop : MovableEntity
{
    // --- Variabile existente raman la fel ---
    [SerializeField] protected List<Transform> patrol_points = new();
    protected int next_patrol_point = 0;
    const float delta_patrol_check = 0.1f;
    FieldOfView child_fov;

    // --- Variabile NOI pentru Animatie ---
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Vector2 lastMoveDirection = Vector2.down;

    private void Awake()
    {
        myAnimator = GetComponentInChildren<Animator>();
        mySpriteRender = GetComponentInChildren<SpriteRenderer>();
        child_fov = GetComponentInChildren<FieldOfView>();

        // --- DEBUG AWAKE ---
        if (myAnimator == null)
            Debug.LogError($"[Cop Debug - {gameObject.name}] Animator component NOT FOUND on children!");
        else
            Debug.Log($"[Cop Debug - {gameObject.name}] Animator component FOUND: {myAnimator.name}");
        // -----------------

        if (mySpriteRender == null) Debug.LogWarning("SpriteRenderer component not found on Cop or its children!", this);
        if (child_fov == null) Debug.LogWarning("FieldOfView component not found on Cop children!", this);

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("Rigidbody2D not found on Cop!", this);

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
    }

    // --- Functiile de Patrulare (NESCHIMBATE) ---
    private void ChangePatrolPoint()
    {
        next_patrol_point = (next_patrol_point + 1) % patrol_points.Count;
        // --- DEBUG PATROL ---
        Debug.Log($"[Cop Debug - {gameObject.name}] Changed patrol point to index: {next_patrol_point}");
        // ------------------
    }

    private void AdjustDirection()
    {
        if (patrol_points == null || patrol_points.Count == 0 || patrol_points[next_patrol_point] == null)
        {
            movement_dir = Vector2.zero;
            // --- DEBUG PATROL ---
            Debug.LogWarning($"[Cop Debug - {gameObject.name}] AdjustDirection - Invalid patrol point or list. Setting movement_dir to zero.");
            // ------------------
            return;
        }
        Vector2 next_point_2D = patrol_points[next_patrol_point].position;
        movement_dir = (next_point_2D - (Vector2)transform.position).normalized;
    }

    private void CheckPatrolPointReached()
    {
        if (patrol_points == null || patrol_points.Count == 0 || patrol_points[next_patrol_point] == null)
        {
            return;
        }
        Vector2 next_point_2D = patrol_points[next_patrol_point].position;
        float distance = (next_point_2D - (Vector2)transform.position).magnitude;

        // --- DEBUG PATROL ---
        // Afisam la intervale mai rare pentru a nu umple consola
        if (Time.frameCount % 60 == 0) // Afiseaza o data pe secunda (aproximativ)
        {
            Debug.Log($"[Cop Debug - {gameObject.name}] Distance to point {next_patrol_point}: {distance.ToString("F2")}");
        }
        // ------------------

        if (distance < delta_patrol_check)
        {
            // --- DEBUG PATROL ---
            Debug.Log($"[Cop Debug - {gameObject.name}] Reached patrol point {next_patrol_point}. Distance: {distance.ToString("F2")}");
            // ------------------
            ChangePatrolPoint();
        }
    }


    protected override void HandleMovement()
    {
        CheckPatrolPointReached();
        AdjustDirection();

        // --- MODIFICARE AICI ---
        // Consideram ca se misca daca are viteza si o directie valida (nu a ajuns inca)
        // Presupunem ca AdjustDirection seteaza movement_dir la zero doar daca nu are punct valid
        bool isCurrentlyMoving = speed > 0.01f && movement_dir.sqrMagnitude > 0.01f;
        // ---------------------

        Debug.Log($"[Cop Debug - {gameObject.name} | Frame {Time.frameCount}] HandleMovement - Speed: {speed.ToString("F2")}, MoveDir: {movement_dir.ToString("F2")}, Calculated isCurrentlyMoving: {isCurrentlyMoving}");

        UpdateAnimatorState(isCurrentlyMoving);

        TranslateEntity();
    }
    // --- UpdateAnimatorState (CU DEBUG) ---
    private void UpdateAnimatorState(bool isMoving)
    {
        if (myAnimator == null)
        {
            Debug.LogError($"[Cop Debug - {gameObject.name}] UpdateAnimatorState called but myAnimator is NULL!");
            return;
        }
        if (mySpriteRender == null) return; // Eroare mai putin critica

        // --- DEBUG Animator ---
        // Verificam starea curenta *inainte* de a o seta
        bool animatorIsMoving = myAnimator.GetBool("IsMoving");
        float animatorMoveX = myAnimator.GetFloat("MoveX");
        float animatorMoveY = myAnimator.GetFloat("MoveY");

        Debug.Log($"[Cop Debug - {gameObject.name} | Frame {Time.frameCount}] UpdateAnimatorState - Received isMoving: {isMoving}. Animator Current State -> IsMoving: {animatorIsMoving}, MoveX: {animatorMoveX.ToString("F2")}, MoveY: {animatorMoveY.ToString("F2")}");
        // --------------------

        // Seteaza parametrul IsMoving
        myAnimator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            myAnimator.SetFloat("MoveX", movement_dir.x);
            myAnimator.SetFloat("MoveY", movement_dir.y);
            lastMoveDirection = movement_dir;

            // --- DEBUG Animator Set ---
            Debug.Log($"[Cop Debug - {gameObject.name} | Frame {Time.frameCount}] Setting Animator -> MoveX: {movement_dir.x.ToString("F2")}, MoveY: {movement_dir.y.ToString("F2")}");
            // ------------------------

            if (movement_dir.x < -0.1f) mySpriteRender.flipX = true;
            else if (movement_dir.x > 0.1f) mySpriteRender.flipX = false;
        }
        else // Idle
        {
            myAnimator.SetFloat("MoveX", lastMoveDirection.x);
            myAnimator.SetFloat("MoveY", lastMoveDirection.y);
            // --- DEBUG Animator Set ---
            Debug.Log($"[Cop Debug - {gameObject.name} | Frame {Time.frameCount}] Setting Animator (Idle) -> MoveX: {lastMoveDirection.x.ToString("F2")}, MoveY: {lastMoveDirection.y.ToString("F2")}");
            // ------------------------
        }
        // --- DEBUG Animator AFTER Set ---
        // Verificam starea *imediat dupa* ce am setat-o (desi poate nu s-a actualizat inca intern complet)
        // animatorIsMoving = myAnimator.GetBool("IsMoving");
        // Debug.Log($"[Cop Debug - {gameObject.name} | Frame {Time.frameCount}] Animator state AFTER set -> IsMoving: {animatorIsMoving}");
        // -----------------------------
    }


    // --- Start (NESCHIMBAT) ---
    void Start()
    {
        movement_dir = Vector2.zero;
        if (speed <= 0 && base_speed > 0) speed = base_speed;

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.SubscribeCop(this);
        }
        else
        {
            Debug.LogWarning("TimeManager Instance not found when Cop tried to subscribe.", this);
        }

        gameObject.tag = "COP";
    }

    // --- Update (NESCHIMBAT - cu logica FoV actualizata) ---
    void Update()
    {
        if (child_fov != null)
        {
            child_fov.UpdateDir(lastMoveDirection); // Folosim ultima directie
        }
    }

    // --- FixedUpdate (NESCHIMBAT) ---
    void FixedUpdate()
    {
        HandleMovement();
    }

    // --- OnFreeze / OnUnfreeze (comentate - foloseste cele din MovableEntity probabil) ---
    /*
    public override void OnFreeze() { ... }
    public override void OnUnfreeze() { ... }
    */

} // Sfarsitul clasei Cop