using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Subtitles, Timer, Inventory

public class UIManager : MonoBehaviour
{
    // UI MANAGER INSTANCE
    public static UIManager Instance { get; private set; } // creates instance for UIManager script

    // INVENTORY VARIABLES
    public GameObject inventoryButton; // button to toggle inventory
    private Animator inventoryButtonAnimator;
    private GameObject inventoryExclamation;
    public GameObject inventoryModelArrow;
    public GameObject modelInfoArrow;
    public Boolean firstInventoryClose;
    public GameObject inventoryUI; // inventory UI background
    public GameObject inventoryGrid; // inventory UI grid
    public GameObject inventoryItemPrefab; // inventory UI item prefab
    public Image modelImage; // inventory UI image for individual model
    public TextMeshProUGUI modelName; // inventory UI text containing model name
    public TextMeshProUGUI modelOrigin; // inventory UI text containing model origin data (which exhibit it belongs to)
    private Boolean inventoryToggled; // if inventory is being displayed
    private int selectedInvIndex = -1; // use this to reference list of models in game manager when releasing models from inventory

    // LASSO VARIABLES
    public GameObject lassoButton; // button to toggle lasso
    private Animator lassoButtonAnimator; // button animator
    private GameObject lassoExclamation; // UI exclamation point for tutorial
    public GameObject lasso; // lasso object with "swipe lasso" script attached
    public GameObject lassoPanel; // panel from which player swipes to throw lasso
    public Boolean lassoToggled; // if lasso is currently enabled
    public bool allModelsCaptured;

    // SUBTITLE VARIABLES
    public TextMeshProUGUI subtitleText; // subtitles
    public TextMeshProUGUI timerText; // current time elapsed

    // TIMER VARIABLES
    public int startMinutes = 15; // game duration in minutes
    public int startSeconds = 0; // game duration in seconds (added to minutes)
    private float remainingTime; // time until game over
    public Vector3 velocity = new Vector3(1, 1, 1); 

    // IMAGE DETECTION / MODEL RELEASE VARIABLES
    public GameObject FlashWarningImage; // warning displayed when image origin is detected
    public Animator warningAnimator; // animator for flashing warning
    public Button ReleaseModelButton; // button to release model from inventory
    public int modelsReturned = 0; // number of models returned
    public bool firstPosterScanned;
    public bool firstModelReturned;

    // AUDIO SOURCE VARIABLES
    public AudioSource dingSound; // ding sound effect
    public AudioSource completionSound; // completion sound effect
    public AudioSource whooshSound; // whoosh sound effect (plays when object is returning to image origin)

    // GAME OVER VARIABLES
    public GameObject successScreen; // success screen that shows when all images returned
    public bool gameOver = false; // if game has ended

    // AWAKE
    void Awake()
    {
        // avoid duplicate singletons
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // instantiates
        Instance = this;
        // optional: persists across scenes
        DontDestroyOnLoad(gameObject);

        // assigns lasso button UI variables
        lassoExclamation = lassoButton.transform.GetChild(0).gameObject;
        lassoButtonAnimator = lassoButton.GetComponent<Animator>();
        inventoryExclamation = inventoryButton.transform.GetChild(0).gameObject;
        inventoryButtonAnimator = inventoryButton.GetComponent<Animator>();

        // default states for variables
        inventoryToggled = false;
        allModelsCaptured = false;
        StopFlashWarning();
        lasso.SetActive(false);
        lassoPanel.SetActive(false);
        lassoButton.SetActive(false);
        lassoButtonAnimator.enabled = false;
        inventoryButton.SetActive(false);
        inventoryButtonAnimator.enabled = false;
        inventoryUI.SetActive(false);
        inventoryModelArrow.SetActive(false);
        modelInfoArrow.SetActive(false);
        firstInventoryClose = false;
        firstPosterScanned = false;
        firstModelReturned = false;
        ToggleReleaseModelButton(false);

    }

    // mode-specific UI variables
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

    // DISPLAY SUBTITLES AND PLAYS AUDIO CLIPS
    public void ShowSubtitles(int subtitleIndex)
    {
        SubtitleSO subtitles = GameManager.Instance.subtitlesDictionary[subtitleIndex];
        Instance.StartCoroutine(Instance.DisplaySubtitles(subtitles.subtitleText, subtitles.subtitlePacing, subtitles.keepLastLineOnScreen));
        GameManager.Instance.audioSource.clip = subtitles.audioClip;
        GameManager.Instance.audioSource.Play();
    }

    // INTRODUCTORY SUBTITLES
    public static void ShowIntroSubtitles()
    {
        Instance.ShowSubtitles(0); // "our data models have escape, please help us!"
        Instance.Invoke("ShowLassoButton", 4.1f);
    }
    // GRAB LASSO SUBTITLES
    public static void ShowLassoGrabSubtitles()
    {
        Instance.ShowSubtitles(1); // "grab your lasso by clicking the button!"
    }
    // THROW LASSO SUBTITLES
    public static void ShowLassoTutorialSubtitles()
    {
        Instance.ShowSubtitles(2);

    }
    // CATCH FIRST MODEL SUBTITLES 
    public static void ShowLassoFirstModelSubtitles()
    {
        Instance.ShowSubtitles(3);
        Instance.Invoke("ShowFirstModel", 3f);
    }
    public void ShowFirstModel()
    {
        foreach (var kvp in GameManager.modelDictionary)
        {
            GameObject modelObject = kvp.Key;
            DataModelInfoSO modelInfo = kvp.Value;
            if (modelInfo.name == "Red Blood Cell")
            {
                modelObject.SetActive(true);
            }
        }
    }
    // INVENTORY INTRO SUBTITLES
    public static void ShowInventoryIntroSubtitles()
    {
        Instance.ShowSubtitles(4);
        Instance.Invoke("ShowOpenInventorySubtitles", 4.9f);
    }
    // OPEN INVENTORY SUBTITLES
    public void ShowOpenInventorySubtitles()
    {
        Instance.ShowInventoryButton();
        Instance.ShowSubtitles(5);
    }

    public void ShowInventorySubtitles()
    {
        Instance.ShowSubtitles(6);
        Instance.Invoke("ShowClickModelSubtitles", 6.2f);
    }

    public void ShowClickModelSubtitles()
    {
        Instance.ShowSubtitles(7);
        inventoryModelArrow.SetActive(true);
    }

    public void ShowModelInformationSubtitles()
    {
        Instance.ShowSubtitles(8);
        modelInfoArrow.SetActive(true);
        Instance.Invoke("CloseInventorySubtitles", 5);
    }

    public void CloseInventorySubtitles()
    {
        Instance.ShowSubtitles(9);
        modelInfoArrow.SetActive(false);
    }

    public void LassoRemainingModelsSubtitles()
    {
        Instance.ShowSubtitles(10);
        Instance.Invoke("ShowRemainingModels", 3);
    }
    public void ShowRemainingModels()
    {
        foreach (var kvp in GameManager.modelDictionary)
        {
            GameObject modelObject = kvp.Key;
            DataModelInfoSO modelInfo = kvp.Value;
            if (modelInfo.name != "Red Blood Cell")
            {
                modelObject.SetActive(true);
            }
        }
    }

    public void AllModelsCapturedSubtitles()
    {
        Instance.ShowSubtitles(11);
        Instance.Invoke("FindPostersSubtitles", 3.4f);
        Instance.disableLassoUI();
    }

    public void FindPostersSubtitles()
    {
        Instance.ShowSubtitles(12);
    }

    public void FirstPosterSubtitles()
    {
        Instance.ShowSubtitles(13);
    }

    public void FirstModelReturnedSubtitles()
    {
        Instance.ShowSubtitles(14);
    }

    // public static void ShowDataSubtitles(DataModelInfoSO modelInfo)
    // {
    //     Instance.StartCoroutine(Instance.DisplaySubtitles(modelInfo.subtitleText, modelInfo.subtitlePacing));

    // }

    // COROUTINE FOR DISPLAYING AND TIMING SUBTITLES
    private IEnumerator DisplaySubtitles(string[] lines, float intervalSeconds, bool persistLastLine)
    {
        foreach (string line in lines)
        {
            subtitleText.text = line;
            yield return new WaitForSeconds(intervalSeconds);
        }
    }

    // 
    public void ShowLassoButton()
    {
        lassoButton.SetActive(true);
        lassoExclamation.SetActive(true);
        lassoButtonAnimator.enabled = true;
        ShowLassoGrabSubtitles();

    }

    public void ShowInventoryButton()
    {
        inventoryButton.SetActive(true);
        inventoryExclamation.SetActive(true);
        inventoryButtonAnimator.enabled = true;

    }

    public void DisplayInventory() // True to set active 
    {
        disableInventoryButtonAnimation();
        if (inventoryToggled)
        {
            inventoryUI.SetActive(false);
            inventoryToggled = false;
            if (!firstInventoryClose)
            {
                firstInventoryClose = true;
                Instance.LassoRemainingModelsSubtitles();
            }
        }
        else
        {
            UpdateInventory(); // For now, update inventory everytime you open it.
            inventoryUI.SetActive(true);
            inventoryToggled = true;
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
        if (GameManager.capturedModels[index].name == "Red Blood Cell")
        {
            if (inventoryModelArrow.activeInHierarchy)
            {
                inventoryModelArrow.SetActive(false);
                Instance.ShowModelInformationSubtitles();
            }
        }
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
        if (lassoToggled)
        {
            lassoPanel.SetActive(true);
            lasso.SetActive(true);
        }
        else
        {
            if (SwipeLasso.lassoReturned)
            {
                lassoPanel.SetActive(false);
                lasso.SetActive(false);
            }
            else
            {
                lassoPanel.SetActive(false);
            }
        }
    }
    

    public void lassoAnimationSmall()
    {
        if (!lassoButtonAnimator.GetBool("button_small"))
        {
            lassoButtonAnimator.SetBool("button_small", true);
            lassoExclamation.SetActive(false);

            ShowLassoTutorialSubtitles();
        }
    }

    public void disableInventoryButtonAnimation()
    {
        if (inventoryButtonAnimator.GetBool("pulsing"))
        {
            inventoryButtonAnimator.SetBool("pulsing", false);
            inventoryExclamation.SetActive(false);

            ShowInventorySubtitles();
        }
    }

    public static void FlashWarning()
    {
        if (Instance.firstPosterScanned == false)
        {
            Instance.firstPosterScanned = true;
            Instance.FirstPosterSubtitles();
        }
        Instance.FlashWarningImage.SetActive(true);
        Instance.warningAnimator.Play("FlashWarning_Anim");
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
                    inventoryToggled = false;
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
        if (modelsReturned == 1 && firstModelReturned == false)
        {
            firstModelReturned = true;
            Instance.FirstModelReturnedSubtitles();
        }

        if (modelsReturned == 3 && !gameOver)
            {
                completionSound.Play();
                successScreen.SetActive(true);
                subtitleText.enabled = false;
                inventoryButton.SetActive(false);
                inventoryUI.SetActive(false);
                gameOver = true;
            }
    }
    

}
