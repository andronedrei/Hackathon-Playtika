using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionMenu : MonoBehaviour
{
    public void LoadScene(string levelName)
    {
        if(levelName == "currentLvl")
        {
            levelName = currentLvl.Instance.CurrentLevel;
            Debug.Log("Current Level: " + currentLvl.Instance.CurrentLevel);
        }
        SceneManager.LoadSceneAsync(levelName);
    }
}
