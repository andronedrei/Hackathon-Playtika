using UnityEngine;
using UnityEngine.SceneManagement; // Necesar pentru a schimba scenele

// Asigura-te ca acest GameObject are un Collider2D
[RequireComponent(typeof(Collider2D))]
public class WinZone : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag-ul asignat primului player")]
    public string player1Tag = "Player1"; // Schimba daca folosesti alt tag

    [Tooltip("Tag-ul asignat celui de-al doilea player")]
    public string player2Tag = "Player2"; // Schimba daca folosesti alt tag

    [Tooltip("Numele exact al scenei de victorie de incarcat")]
    public string winSceneName = "WinScene";

    // Variabile private pentru a urmari cine e inauntru
    private bool isPlayer1Inside = false;
    private bool isPlayer2Inside = false;
    private bool sceneIsLoading = false; // Previne incarcari multiple

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        // Fortam collider-ul sa fie Trigger, pentru siguranta
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider pe {gameObject.name} nu era setat pe 'Is Trigger'. S-a fortat acum.", this);
            col.isTrigger = true;
        }
    }

    // Se apeleaza cand un obiect cu Collider2D intra in trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificam daca scena este deja in curs de incarcare
        if (sceneIsLoading) return;

        // Verificam tag-ul obiectului care a intrat
        if (other.CompareTag(player1Tag))
        {
            Debug.Log("Player 1 entered Win Zone.");
            isPlayer1Inside = true;
        }
        else if (other.CompareTag(player2Tag))
        {
            Debug.Log("Player 2 entered Win Zone.");
            isPlayer2Inside = true;
        }

        // Verificam daca ambii playeri sunt acum inauntru
        CheckWinCondition();
    }

    // Se apeleaza cand un obiect cu Collider2D iese din trigger
    private void OnTriggerExit2D(Collider2D other)
    {
         // Verificam daca scena este deja in curs de incarcare
        if (sceneIsLoading) return;

        // Verificam tag-ul obiectului care a iesit
        if (other.CompareTag(player1Tag))
        {
            Debug.Log("Player 1 exited Win Zone.");
            isPlayer1Inside = false;
        }
        else if (other.CompareTag(player2Tag))
        {
            Debug.Log("Player 2 exited Win Zone.");
            isPlayer2Inside = false;
        }

        // Nu mai verificam conditia de win la iesire, doar resetam flag-urile
    }

    // Metoda care verifica daca ambii playeri sunt inauntru si incarca scena
    private void CheckWinCondition()
    {
        // Daca ambii sunt inauntru SI nu am inceput deja sa incarcam scena
        if (isPlayer1Inside && isPlayer2Inside && !sceneIsLoading)
        {
            sceneIsLoading = true; // Marcheaza ca incepem incarcarea
            Debug.Log("WIN CONDITION MET! Both players inside. Loading scene: " + winSceneName);

            // Incarca scena de victorie
            // Asigura-te ca scena "WinScene" exista si este adaugata in Build Settings!
            SceneManager.LoadSceneAsync("horo(win)");
        }
    }
}