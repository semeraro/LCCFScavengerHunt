using System;
using System.Collections;
using TMPro;
using UnityEngine;

//Subtitles, Timer, Inventory

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    

    public GameObject inventoryUI;
    public GameObject inventoryButton;
    private Boolean isInvDisplayed;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI countdownText; 

    public Boolean lassoToggled;
    public GameObject lasso;
    public GameObject lassoPanel;
    

    void Awake()
    {
        Debug.Log("UI Manager Awake");
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Avoid duplicate singletons
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persists across scenes

        isInvDisplayed = false;

        
    }

    public static void SetTourModeUI()
    {
        Debug.Log("PlayMode Tour"); // for some reason this doesnt work
            // Load or hide specific mode UI
        Instance.inventoryUI.SetActive(false);
        Instance.inventoryButton.SetActive(false);
        Instance.lasso.SetActive(false);
        Instance.lassoPanel.SetActive(false);
    }
    public static void SetGameModeUI()
    {
        Instance.lassoToggled = false;
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

    
    public void UpdateInventory()
    {
    
    }
    

    public static void StartCountdown()
    {
        
    }
    public void lassoToggle()
    {
        lassoToggled = !lassoToggled;
        lassoPanel.SetActive(lassoToggled);
        lasso.SetActive(lassoToggled);
    }
    
}
