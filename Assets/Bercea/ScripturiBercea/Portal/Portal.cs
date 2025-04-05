using UnityEngine;
using System.Collections; // Needed for Coroutine

// Ensure this GameObject has a Collider2D
[RequireComponent(typeof(Collider2D))]
public class SimplePortal : MonoBehaviour // CLASS DEFINITION STARTS HERE
{
    [Header("Portal Settings")]
    [Tooltip("Drag the Transform of the DESTINATION GameObject here (the other portal's exit point).")]
    public Transform destinationTransform; // Where to teleport to

    [Tooltip("The tag of the object(s) that can use the portal.")]
    public string playerTag = "Player"; // Change if your players have different tags

    private Collider2D portalCollider;
    private bool canTeleport = true; // Flag to prevent rapid multi-teleports

    void Awake()
    {
        portalCollider = GetComponent<Collider2D>();

        // Very important: Ensure this collider is a Trigger!
        if (!portalCollider.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} was not set to 'Is Trigger'. Forcing it now.", this);
            portalCollider.isTrigger = true;
        }
    }

    // Main method that detects entry
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if teleport is allowed and if the entering object has the correct tag
        if (canTeleport && other.CompareTag(playerTag))
        {
            // Check if the destination is set in the Inspector
            if (destinationTransform != null)
            {
                // Teleport the object (the player)
                TeleportObject(other.gameObject); // Send the player's GameObject

                // --- Prevent instant re-teleport from the destination portal ---
                // Look for the SimplePortal script on the destination object itself or its parent if needed
                // This assumes the destinationTransform points directly to the object with the *other* portal script
                SimplePortal destinationPortalScript = destinationTransform.GetComponent<SimplePortal>();
                if (destinationPortalScript != null)
                {
                    // Temporarily disable teleporting at the destination
                    destinationPortalScript.StartCoroutine(destinationPortalScript.TeleportCooldown());
                }
                else {
                    // Optional: Check parent if structure is different
                    // SimplePortal parentPortalScript = destinationTransform.GetComponentInParent<SimplePortal>();
                    // if (parentPortalScript != null) parentPortalScript.StartCoroutine(parentPortalScript.TeleportCooldown());
                }
            }
            else
            {
                Debug.LogError($"Destination is not set for portal {gameObject.name}!", this);
            }
        }
    }

    // Function that performs the teleportation
    private void TeleportObject(GameObject objectToTeleport)
    {
        // Move the object to the destination's position
        // Using .position for world coordinates
        objectToTeleport.transform.position = destinationTransform.position;
        Debug.Log($"{objectToTeleport.name} teleported from {gameObject.name} to {destinationTransform.name}");

        // Stop the player's velocity to prevent flying out of the new portal
        Rigidbody2D playerRb = objectToTeleport.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }
    }

    // Coroutine to add a small delay before a portal can teleport AGAIN
    // Prevents infinite loops if portals are very close
    public IEnumerator TeleportCooldown()
    {
        canTeleport = false; // Block teleporting from THIS portal
        // Debug.Log($"Portal {gameObject.name} cooldown started.");
        yield return new WaitForSeconds(0.5f); // Wait half a second
        canTeleport = true; // Allow teleporting again
        // Debug.Log($"Portal {gameObject.name} cooldown finished.");
    }

    // Optional: Visualization in Editor
    void OnDrawGizmos()
    {
        if (destinationTransform != null)
        {
            Gizmos.color = Color.cyan;
            // Draw a line from this portal's trigger to the destination point
            Gizmos.DrawLine(transform.position, destinationTransform.position);
            // Draw a small sphere at the destination point for visibility
            Gizmos.DrawWireSphere(destinationTransform.position, 0.5f);
        }
        else
        {
             // Draw a red sphere if destination is not set
             Gizmos.color = Color.red;
             Gizmos.DrawWireSphere(transform.position, 0.6f);
        }
    }

} // <<< MAKE SURE THIS IS THE FINAL CLOSING BRACE FOR THE CLASS! NO CODE AFTER THIS!