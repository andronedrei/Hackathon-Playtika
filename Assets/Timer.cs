using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] protected Image backgroundImagealbg;
    // private float countdownTime = 10f; // Countdown time in seconds

    // Hard coded sprites
    [SerializeField] protected Sprite normalBgSprite;
    [SerializeField] protected Sprite warningBgSprite;

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

    public void SetTimer(float time, bool freeze_img) {
        currentTime = time;

        if (freeze_img)
        {
            backgroundImagealbg.sprite = normalBgSprite;
        }
        else
        {
            backgroundImagealbg.sprite = warningBgSprite;
        }
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

