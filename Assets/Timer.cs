using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    // private float countdownTime = 10f; // Countdown time in seconds

    private float currentTime;

    void Start()
    {
        if (countdownText == null)
        {
            Debug.LogError("CountdownText is not assigned!");
            return;
        }

        // currentTime = countdownTime;
        // UpdateCountdownText();
    }

    public void SetTimer(float time) {
        currentTime = time;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateCountdownText();
        }
        else
        {
            currentTime = 0;
            UpdateCountdownText();
            // Optionally, trigger an event when the countdown reaches zero
            TimeManager.Instance.TtriggerNextAction();
        }
    }

    void UpdateCountdownText()
    {
        int seconds = Mathf.FloorToInt(currentTime);
        int milliseconds = Mathf.FloorToInt((currentTime - seconds) * 100);

        countdownText.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);
    }
}

