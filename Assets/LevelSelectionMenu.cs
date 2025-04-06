using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionMenu : MonoBehaviour
{
    public void LoadScene(string levelName)
    {
        // {
        //     levelName = currentLvl.Instance.CurrentLevel;
        //     Debug.Log("Current Level: " + currentLvl.Instance.CurrentLevel);
        // }  if(levelName == "currentLvl")
      
        SceneManager.LoadSceneAsync(levelName);
    }
}
