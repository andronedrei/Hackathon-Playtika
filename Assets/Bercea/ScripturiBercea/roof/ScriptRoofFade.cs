using UnityEngine;
using System.Collections; // Needed for Coroutines
using System.Collections.Generic; // Needed for HashSet

public class SmoothMultiPlayerRoofFader : MonoBehaviour // Renamed for clarity
{
    [Header("Target Renderer")]
    [Tooltip("Assign the SpriteRenderer OR TilemapRenderer of the roof here")]
    public Renderer targetRenderer;

    [Header("Fading Settings")]
    [Tooltip("The alpha value when faded (0 = fully transparent, 1 = fully opaque)")]
    [Range(0f, 1f)]
    public float fadedAlpha = 0.1f; // Slightly visible is often better
    [Tooltip("How long the fade takes in seconds")]
    public float fadeDuration = 0.5f;

    [Header("Detection Settings")]
    [Tooltip("Tag for the first player")]
    public string player1Tag = "Player1";
    [Tooltip("Tag for the second player")]
    public string player2Tag = "Player2";

    private float _opaqueAlpha = 1.0f;
    private Color _originalColor;
    private Coroutine _fadeCoroutine;

    // Use HashSet for efficient tracking of unique players inside
    private HashSet<Collider2D> _playersInside = new HashSet<Collider2D>();

    void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>(); // Try to find it automatically
            if (targetRenderer == null)
            {
                Debug.LogError("SmoothMultiPlayerRoofFader: No Renderer assigned or found!", this);
                enabled = false; // Disable script if no renderer
                return;
            }
        }

        // Safety check for material
        if (targetRenderer.material == null)
        {
            Debug.LogError("SmoothMultiPlayerRoofFader: Target Renderer has no material!", this);
            enabled = false;
            return;
        }

        // Store the original color and its alpha value
        _originalColor = targetRenderer.material.color;
        _opaqueAlpha = _originalColor.a; // Use the material's starting alpha

        // Ensure roof starts opaque and the tracking list is clear
        SetAlpha(_opaqueAlpha); // Set initial state directly
        _playersInside.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider entering belongs to Player1 OR Player2
        if (other.CompareTag(player1Tag) || other.CompareTag(player2Tag))
        {
            // Try to add the player. If successful (player wasn't already inside)...
            if (_playersInside.Add(other))
            {
                // ...and if this is the VERY FIRST player entering...
                if (_playersInside.Count == 1)
                {
                    // Debug.Log($"First player ({other.name}) entered. Fading OUT.", this);
                    // Stop any previous fade and start fading OUT (to transparent)
                    if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = StartCoroutine(FadeTo(fadedAlpha, fadeDuration));
                }
                // Else: another player entered, but one was already inside, so roof should already be fading/faded out. Do nothing.
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collider leaving belongs to Player1 OR Player2
        if (other.CompareTag(player1Tag) || other.CompareTag(player2Tag))
        {
            // Try to remove the player. If successful (player was actually inside)...
            if (_playersInside.Remove(other))
            {
                // ...and if the count is now ZERO (meaning the LAST player just left)...
                if (_playersInside.Count == 0)
                {
                    // Debug.Log($"Last player ({other.name}) exited. Fading IN.", this);
                    // Stop any previous fade and start fading IN (to opaque)
                    if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = StartCoroutine(FadeTo(_opaqueAlpha, fadeDuration));
                }
                // Else: a player left, but others remain inside. Do nothing, roof stays faded out.
            }
        }
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (targetRenderer == null || targetRenderer.material == null) yield break; // Safety check

        Color currentColor = targetRenderer.material.color;
        float startAlpha = currentColor.a;
        float timer = 0f;

        // Avoid division by zero if duration is instant
        if (duration <= 0f)
        {
            SetAlpha(targetAlpha);
            _fadeCoroutine = null;
            yield break; // Exit coroutine
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress); // Smooth interpolation
            SetAlpha(newAlpha);
            yield return null; // Wait for the next frame
        }

        // Ensure the final alpha is set exactly after the loop
        SetAlpha(targetAlpha);
        _fadeCoroutine = null; // Mark coroutine as finished
    }

    void SetAlpha(float alpha)
    {
        if (targetRenderer == null || targetRenderer.material == null) return; // Extra safety

        // Get the current color, change only the alpha, and apply it back
        Color newColor = targetRenderer.material.color;
        newColor.a = Mathf.Clamp01(alpha); // Ensure alpha is between 0 and 1
        targetRenderer.material.color = newColor;
    }

    // Optional: Good practice to reset visibility if the roof object gets disabled
    void OnDisable()
    {
        // Stop any running fade
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        // Reset alpha to opaque if renderer/material exists
        if (targetRenderer != null && targetRenderer.material != null)
        {
            SetAlpha(_opaqueAlpha);
        }
        _playersInside.Clear(); // Clear tracking
    }
}