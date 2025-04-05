using UnityEngine;

public class currentLvl : MonoBehaviour
{
    public static currentLvl Instance { get; private set; }

    public string CurrentLevel { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("currentLvl Instance has been set.");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
