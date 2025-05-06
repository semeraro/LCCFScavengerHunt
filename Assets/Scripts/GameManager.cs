using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;



public class GameManager : MonoBehaviour
{
    public GameObject image1Prefab;
    public GameObject image2Prefab;
    public GameObject image1CheckMark;
    public GameObject image2CheckMark;
    public GameObject orbCheckMark;
    public GameObject cubeCheckMark;
    public GameObject lasso;
    public GameObject lassoPanel;
    bool image1Tracked = false;
    bool image2Tracked = false;
    public GameObject uiBox;
    public GameObject winScreen;
    public AudioSource dingSound;
    public AudioSource completionSound;
    public SwipeLasso swipe_lasso_script;
    public bool lassoToggled;


    public SpeechBubbleSO image1SpeechSO; //Convert to an array list eventually.
    private AudioSource audioSource;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // gets the AudioSource on the same object
        lassoToggled = false;
    }

    // Update is called once per frame
    void Update()
    {
       if (!image1Tracked) {
            if(image1Prefab.activeInHierarchy == true)
            {
                image1Tracked = true;
                image1CheckMark.SetActive(true);
                dingSound.Play();


                // UIManager.ShowSubtitles(image1SpeechSO);              
                // audioSource.clip = image1SpeechSO.audioClip;
                // audioSource.Play();


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
    public void lassoToggle()
    {
        lassoToggled = !lassoToggled;
        lassoPanel.SetActive(lassoToggled);
        lasso.SetActive(lassoToggled);
    }
}
