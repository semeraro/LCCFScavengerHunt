using System;
using System.Collections;
using TMPro;
using UnityEngine;

//Subtitles, Timer, Inventory

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject inventoryUI;
    private Boolean isInvDisplayed;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI countdownText; 
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Avoid duplicate singletons
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persists across scenes
    }

    
    public static void ShowSubtitles(SpeechBubbleSO speechBubble)
    {
        Instance.StartCoroutine(Instance.DisplaySubtitles(speechBubble.subtitleText, speechBubble.subtitlePacing));
    }

    private IEnumerator DisplaySubtitles(string[] lines, float intervalSeconds)
    {
        foreach (string line in lines)
        {
            subtitleText.text = line;
            yield return new WaitForSeconds(intervalSeconds);
        }

        // Optional: clear text after subtitles finish
        subtitleText.text = "---";
    }
    public void DisplayInventory() // True to set active 
    {
        if (isInvDisplayed)
        {
            inventoryUI.SetActive(false);
            isInvDisplayed = false;
        }
        else
        {
            inventoryUI.SetActive(true);
            isInvDisplayed = true;
        }
    }
    

    public static void StartCountdown()
    {
        
    }
    
}
