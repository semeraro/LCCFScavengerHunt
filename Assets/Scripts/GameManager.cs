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
    bool image1Tracked = false;
    bool image2Tracked = false;
    public GameObject uiBox;
    public GameObject winScreen;
    public AudioSource dingSound;
    public AudioSource completionSound;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       if (!image1Tracked) {
            if(image1Prefab.active)
            {
                image1Tracked = true;
                image1CheckMark.SetActive(true);
                dingSound.Play();
            }
       }

       if (!image2Tracked) {
            if(image2Prefab.active)
            {
                image2Tracked = true;
                image2CheckMark.SetActive(true);
                dingSound.Play();
            }
       }

        else if (image1Tracked && image2Tracked) {
            winScreen.SetActive(true);
            uiBox.SetActive(false);
            completionSound.Play();

        }
    }
}
