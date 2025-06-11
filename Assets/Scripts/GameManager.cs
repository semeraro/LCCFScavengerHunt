using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using Imagine.WebAR;
using NUnit.Framework;



public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }

    public GameObject image1Prefab;
    public GameObject image2Prefab;
    public GameObject image1CheckMark;
    public GameObject image2CheckMark;
    public GameObject orbCheckMark;
    public GameObject cubeCheckMark;
    bool image1Tracked = false;
    bool image2Tracked = false;
    public bool allModelsCaptured = false;
    public GameObject uiBox;
    public GameObject winScreen;
    public AudioSource dingSound;
    public AudioSource completionSound;
    public AudioSource capturedAudio;
    public SwipeLasso swipe_lasso_script;
    public UIManager uiManager;



    public AudioSource audioSource;


    //Central control for instances such as UI Manager to reference for specific modes.
    [SerializeField]
    public enum PlayMode
    {
        Game,
        Tour
    }
    [SerializeField]
    public PlayMode playMode; // Shows up as a dropdown in the Inspector
    

    public List<ModelEntry> modelEntries; // Shows in Inspector
    public static Dictionary<GameObject, DataModelInfoSO> modelDictionary = new Dictionary<GameObject, DataModelInfoSO>();
    public List<SubtitleSO> subtitlesDictionary = new List<SubtitleSO>();

    [SerializeField]
    public static List<DataModelInfoSO> capturedModels = new List<DataModelInfoSO>();

    public static GameObject activeDataOrigin;
    void Awake()
    {
        Debug.Log("Game Manager Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Avoid duplicate singletons
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persists across scenes

        
        if (playMode == PlayMode.Game)
        {
            // Load or hide specific mode UI
            //UIManager.SetGameModeUI(); //Need to debug why this line doesnt work
            
        }

        if (playMode == PlayMode.Tour)
        {
            UIManager.SetTourModeUI();
        }

        foreach (var entry in modelEntries)
        {
            Debug.Log("Hi");
            if (entry.modelObject != null && !modelDictionary.ContainsKey(entry.modelObject))
            {
                entry.modelInfo.isBeingLassoed = false;
                entry.modelInfo.isTracked = false;
                entry.modelInfo.isReturning = false;
                entry.modelInfo.isReturned = false;
                entry.modelInfo.isCaptured = false;
                modelDictionary.Add(entry.modelObject, entry.modelInfo);
                Debug.Log("Key (GameObject): " + entry.modelObject.name + ", Value (DataModelInfoSO): " + entry.modelInfo.name);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        audioSource = GetComponent<AudioSource>(); // gets the AudioSource on the same object
        UIManager.ShowIntroSubtitles();
    }
    public static PlayMode GetPlayMode()
    {
        return Instance.playMode;
    }

    // Update is called once per frame
    void Update()
    {   
        if(playMode == PlayMode.Game)
        {
            GameModeUpdate();
        }
        if(playMode == PlayMode.Tour)
        {
            TourModeUpdate();
        }
       
    }

    void GameModeUpdate()
    {

        // New Structure
        foreach (var kvp in modelDictionary)
        {
            GameObject modelObject = kvp.Key;
            DataModelInfoSO modelInfo = kvp.Value;

            if (!modelInfo.isTracked)
            {
                if (modelObject.activeInHierarchy == true)
                {
                    modelInfo.isTracked = true;
                    //dingSound.Play();


                    // UIManager.ShowDataSubtitles(modelInfo);
                    // audioSource.clip = modelInfo.audioClip;
                    // audioSource.Play();


                }
            }

        }
        if (capturedModels.Count == 3 && !allModelsCaptured)
        {
            allModelsCaptured = true;
            capturedAudio.Play();
            uiManager.disableLassoUI();
                
        }
    }
    void TourModeUpdate()
    {
        
    }
}




[System.Serializable]
public class ModelEntry
{
    public GameObject modelObject;
    public DataModelInfoSO modelInfo;
}
