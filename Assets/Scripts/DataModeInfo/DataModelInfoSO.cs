using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using Imagine.WebAR;
using System;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DataModelInfoSO", menuName = "Scriptable Objects/DataModelInfoSO")]
public class DataModelInfoSO : ScriptableObject
{
    public Sprite image; // 2D image for the inventory display.
    public GameObject modelPrefab; // Access this prefab in case you want to reinstantiate it in the scene after you've captured it... @Hailey / Ayon
    
    public Boolean isTracked;
    public Boolean isBeingLassoed; // if it is currently being lassoed
    public Boolean isCaptured; // if it is false then it is released.
    public Boolean isReturned; // Relevant to the game mode ONLY. If it is returned by user to the correct station.
    public String origin; //The station it is originated from. This could later become an enum that stores specific station locations. i.e. Station 1, 2, 3. For modularity purposes and change the name of the STations in their own SO.
    public String name; // The Name of the data model.

    // Combining the og SpeechBubbleSO into this one big SO.
    [Tooltip("Array of each line of subtitles. In the future, you can create a method to read from a text file so you dont have to manual input this.")]
    public String[] subtitleText; 
    
    [Tooltip("Pauses between switching between subtitle text.")]
    public float subtitlePacing; 
    public AudioClip audioClip; 


    
    
}
