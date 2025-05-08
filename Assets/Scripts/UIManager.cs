using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Subtitles, Timer, Inventory

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    
    

    public GameObject inventoryUI;
    public GameObject inventoryGrid;
    public GameObject inventoryItemPrefab;
    public Image modelImage;
    public TextMeshProUGUI modelName;
    public TextMeshProUGUI modelOrigin;

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

    //Set mode specific UI here!
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
    
    public static void ShowSubtitles(DataModelInfoSO modelInfo)
    {
        Instance.StartCoroutine(Instance.DisplaySubtitles(modelInfo.subtitleText, modelInfo.subtitlePacing));
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
            UpdateInventory(); // For now, update inventory everytime you open it.
            inventoryUI.SetActive(true);
            isInvDisplayed = true;
        }
    }


    
    public void UpdateInventory()
    {

        // Clear existing child objects
        foreach (Transform child in inventoryGrid.transform)
        {
            Destroy(child.gameObject);
        }

        //Filter dictionary to create a list of captured model SOs.
        GameManager.capturedModels =  GameManager.modelDictionary
        .Where(kvp => kvp.Value.isCaptured)
        .Select(kvp => kvp.Value)
        .ToList();

        if (GameManager.capturedModels != null && GameManager.capturedModels.Count > 0)
        {
            // Populate with captured models
            int index = 0;

                foreach (var modelSO in GameManager.capturedModels)
                {
                    DataModelInfoSO modelInfo = modelSO;

                    if (modelInfo.isCaptured)
                    {
                        GameObject item = Instantiate(inventoryItemPrefab, inventoryGrid.transform);

                        Transform icon = item.transform.Find("Image");
                        if (icon != null)
                        {
                            icon.GetComponent<Image>().sprite = modelInfo.image;
                        }

                        // Example: pass the index to a button click event
                        Button button = item.GetComponent<Button>();
                        if (button != null)
                        {
                            int capturedIndex = index; // Prevent closure capture issue
                            button.onClick.AddListener(() => SetModelStats(capturedIndex));
                        }

                        index++;
                    }
                }
        }
        else
        {

        }
        


        
    }
    public void SetModelStats(int index)
    {
        modelImage.sprite = GameManager.capturedModels[index].image;
        modelName.text = GameManager.capturedModels[index].name;
        modelOrigin.text = GameManager.capturedModels[index].origin;
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
