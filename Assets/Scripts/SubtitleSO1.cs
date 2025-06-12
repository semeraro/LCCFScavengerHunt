using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using Imagine.WebAR;
using System;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SubtitleSO", menuName = "Scriptable Objects/SubtitleSO")]
public class SubtitleSO : ScriptableObject
{
    // Combining the og SpeechBubbleSO into this one big SO.
    [Tooltip("Array of each line of subtitles. In the future, you can create a method to read from a text file so you dont have to manual input this.")]
    public String[] subtitleText; 
    
    [Tooltip("Pauses between switching between subtitle text.")]
    public float subtitlePacing;
    public bool keepLastLineOnScreen;
    public AudioClip audioClip; 


    
    
}