using UnityEngine;

public class LvlManager : MonoBehaviour
{
    public static LvlManager Instance { get; private set; }

    public string CurrentLevel { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
