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
    public GameObject lassoButton;
    private Boolean isInvDisplayed;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI timerText;
    public int startMinutes = 15;
    public int startSeconds = 0;
    private float remainingTime;
    public Vector3 velocity = new Vector3(1, 1, 1);


    public Boolean lassoToggled;
    public GameObject lasso;
    public GameObject lassoPanel;

    //Use this to reference list of models in Game Manager when Releasing Models
    private int selectedInvIndex = -1;

    public GameObject FlashWarningImage;
    public Animator animator;
    public Button ReleaseModelButton;
    public AudioSource dingSound;
    public AudioSource completionSound;
    public AudioSource whooshSound;
    public GameObject successScreen;
    public int modelsReturned = 0;
    public bool gameOver = false;


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


        StopFlashWarning();

        ToggleReleaseModelButton(false);

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
        GameManager.capturedModels = GameManager.modelDictionary
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
                    Button button = item.transform.Find("Button").GetComponent<Button>();
                    if (button != null)
                    {
                        int capturedIndex = index; // Prevent closure capture issue
                        Debug.Log(capturedIndex);
                        button.onClick.AddListener(() => SetModelStats(capturedIndex));
                    }

                    index++;
                }
            }
        }

    }

    public void disableLassoUI()
    {
        lassoButton.SetActive(false);
        lasso.SetActive(false);
        lassoPanel.SetActive(false);
    }
    public void SetModelStats(int index)
    {
        selectedInvIndex = index;
        Debug.Log(selectedInvIndex); // Why is this always 1?
        modelImage.sprite = GameManager.capturedModels[index].image;
        modelName.text = GameManager.capturedModels[index].name;
        modelOrigin.text = GameManager.capturedModels[index].origin;

    }

    public static void StartTimer() // Use after onboarding sequence
    {
        Instance.remainingTime = Instance.startMinutes * 60 + Instance.startSeconds;
        Instance.StartCoroutine(Instance.UpdateTimer());

    }
    private IEnumerator UpdateTimer()
    {
        while (remainingTime > 0)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        // Time's up
        timerText.text = "00:00";
    }
    public void lassoToggle()
    {
        lassoToggled = !lassoToggled;
        lassoPanel.SetActive(lassoToggled);
        lasso.SetActive(lassoToggled);
    }

    public static void FlashWarning()
    {
        Instance.FlashWarningImage.SetActive(true);
        Instance.animator.Play("FlashWarning_Anim");
    }
    public static void StopFlashWarning()
    {
        Instance.FlashWarningImage.SetActive(false);
    }
    public static void ToggleReleaseModelButton(bool isEnabled)
    {
        Instance.ReleaseModelButton.interactable = isEnabled;
    }
    public void CheckIfCorrectReleasedModel()
    {
        if (GameManager.capturedModels[selectedInvIndex] == GameManager.activeDataOrigin.GetComponent<MissingDataOrigin>().correctModel)
        {
            Debug.Log("Released Correct Model!");
            whooshSound.Play();
            GameManager.capturedModels[selectedInvIndex].isReturning = true;
            GameManager.capturedModels[selectedInvIndex].isCaptured = false;

            foreach (var kvp in GameManager.modelDictionary)
            {
                GameObject modelObject = kvp.Key;
                DataModelInfoSO modelInfo = kvp.Value;

                if (modelInfo.name == GameManager.capturedModels[selectedInvIndex].name)
                {
                    modelObject.SetActive(true);
                    inventoryUI.SetActive(false);
                    isInvDisplayed = false;
                }
            }
            UpdateInventory();
        }
    }

    void Update()
    {
        foreach (var kvp in GameManager.modelDictionary)
        {
            GameObject modelObject = kvp.Key;
            DataModelInfoSO modelInfo = kvp.Value;

            if (modelInfo.isReturning)
            {
                float distanceToImage = Vector3.Distance(modelObject.transform.position, GameManager.activeDataOrigin.GetComponent<MissingDataOrigin>().transform.position);
                if (distanceToImage > 0.05)
                {
                    modelObject.transform.position = Vector3.SmoothDamp(modelObject.transform.position, GameManager.activeDataOrigin.GetComponent<MissingDataOrigin>().transform.position, ref velocity, 0.3f);
                }
                else
                {
                    modelObject.transform.parent = GameManager.activeDataOrigin.GetComponent<MissingDataOrigin>().transform.parent;
                    GameManager.activeDataOrigin.GetComponent<MissingDataOrigin>().enabled = false;
                    modelInfo.isReturning = false;
                    modelInfo.isReturned = true;
                    dingSound.Play();
                    modelsReturned += 1;
                }
            }
        }

        if (modelsReturned == 3 && !gameOver)
        {
            completionSound.Play();
            successScreen.SetActive(true);
            inventoryButton.SetActive(false);
            inventoryUI.SetActive(false);
            gameOver = true;
        }
    }
    

}
