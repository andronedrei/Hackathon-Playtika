using Unity.VisualScripting;
using UnityEngine;

// clasa tip singleton gestionare aspecte "high-level"
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameManager");
                _instance = obj.AddComponent<GameManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private bool freezed = false;
    //private bool freezed = false;

    // ruleaza inainte de primul frame
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this; // se asigura ca exista o onstanta
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // facem gravitatea 0 deoarece e un joc top-down
        Physics2D.gravity = Vector2.zero;
    }

    private void Update()
    {
        // DEBUG - poti comenta
        //TimeManager.Instance.MyDebug();

        // FREEZE sau UNFREEZE din "space"
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (freezed) {
                TimeManager.Instance.Unfreeze();
            } 
            if (!freezed) {
                TimeManager.Instance.Freeze();
            }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     if (freezed) {
        //         TimeManager.Instance.Unfreeze();
        //     } 
        //     if (!freezed) {
        //         TimeManager.Instance.Freeze();
        //     }

            freezed = !freezed;
        }
        //     freezed = !freezed;
        // }
    }
}
