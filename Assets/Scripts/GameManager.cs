using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;



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
    public GameObject uiBox;
    public GameObject winScreen;
    public AudioSource dingSound;
    public AudioSource completionSound;
    public SwipeLasso swipe_lasso_script;


    public SpeechBubbleSO image1SpeechSO; //Convert to an array list eventually.
    private AudioSource audioSource;

    //Central control for instances such as UI Manager to reference for specific modes.
    [SerializeField]
    public enum PlayMode
    {
        Game,
        Tour
    }
    [SerializeField]
    public PlayMode playMode; // Shows up as a dropdown in the Inspector
    
    
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

        
        if (playMode == GameManager.PlayMode.Game)
        {
            // Load or hide specific mode UI
            UIManager.SetGameModeUI();
            
        }

        if (playMode == GameManager.PlayMode.Tour)
        {
            UIManager.SetTourModeUI();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // gets the AudioSource on the same object
        
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
        if (!image1Tracked) {
            if(image1Prefab.activeInHierarchy == true)
            {
                image1Tracked = true;
                image1CheckMark.SetActive(true);
                dingSound.Play();


                UIManager.ShowSubtitles(image1SpeechSO);              
                audioSource.clip = image1SpeechSO.audioClip;
                audioSource.Play();


            }
       }

       if (!image2Tracked) {
            if(image2Prefab.activeInHierarchy == true)
            {
                image2Tracked = true;
                image2CheckMark.SetActive(true);
                dingSound.Play();
            }
       }

        else if (image1Tracked && image2Tracked) {
            //winScreen.SetActive(true);
            //uiBox.SetActive(false);
            completionSound.Play();

        }

        if (swipe_lasso_script.orbCaptured)
        {
            orbCheckMark.SetActive(true);
            dingSound.Play();
        }

        if (swipe_lasso_script.cubeCaptured)
        {
            cubeCheckMark.SetActive(true);
            dingSound.Play();
        }

        if (swipe_lasso_script.orbCaptured && swipe_lasso_script.cubeCaptured)
        {
            completionSound.Play();
            winScreen.SetActive(true);
            uiBox.SetActive(false);
        }
    }
    void TourModeUpdate()
    {

    }
}
