using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionMenu : MonoBehaviour
{
    public void LoadScene(string levelName)
    {
        SceneManager.LoadSceneAsync(levelName);
    }
}
