using UnityEngine;
using System.Collections; // Ne trebuie pentru IEnumerator

public enum PoolThiefType { Type_1, Type_2 }
public enum ControlType { Arrows, WASD }

public class Player : MovableEntity
{
    [SerializeField] protected PoolThiefType pool_thief_type;
    [SerializeField] protected ControlType control_type;

    protected KeyCode[] movement_keys = new KeyCode[4];

    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Vector2 lastMovementDirection = Vector2.down;

    AudioSource audioSource;

    private Coroutine fadeCoroutine;
    [SerializeField] private float fadeDuration = 0.3f;

    private void Awake()
    {
        myAnimator = GetComponentInChildren<Animator>();
        mySpriteRender = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (myAnimator == null) Debug.LogError("Animator component not found!", this);
        if (mySpriteRender == null) Debug.LogError("SpriteRenderer component not found!", this);
    }

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
        {
            mySpriteRender.flipX = false;
            movement_dir.x += 1.0f;
        }

        if (left)
        {
            mySpriteRender.flipX = true;
            movement_dir.x -= 1.0f;
        }

        movement_dir = movement_dir.normalized;

        TranslateEntity();
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (myAnimator == null) return;

        bool isMoving = movement_dir.sqrMagnitude > 0.01f;
        myAnimator.SetBool("IsMoving", isMoving);

        bool wasMoving = audioSource.isPlaying;

        if (isMoving)
        {
            myAnimator.SetFloat("MoveX", movement_dir.x);
            myAnimator.SetFloat("MoveY", movement_dir.y);
            lastMovementDirection = movement_dir;

            if (!wasMoving)
            {
                StartFadeIn();
            }
        }
        else
        {
            myAnimator.SetFloat("MoveX", lastMovementDirection.x);
            myAnimator.SetFloat("MoveY", lastMovementDirection.y);

            if (wasMoving)
            {
                StartFadeOut();
            }
        }
    }

    private void StartFadeIn()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn());
    }

    private void StartFadeOut()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        audioSource.volume = 0f;
        if (!audioSource.isPlaying) audioSource.Play();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1f;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    void Start()
    {
        speed = base_speed;

        SubscribeToTimeManager();
        SetupKeys();
    }

    private void SetupKeys()
    {
        switch (control_type)
        {
            case ControlType.WASD:
                movement_keys[0] = KeyCode.W; movement_keys[1] = KeyCode.S;
                movement_keys[2] = KeyCode.A; movement_keys[3] = KeyCode.D;
                break;
            default:
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

    void FixedUpdate()
    {
        HandleMovement();
    }
}
