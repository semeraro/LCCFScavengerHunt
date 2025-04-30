using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeechBubbleSO", menuName = "Scriptable Objects/SpeechBubbleSO")]
public class SpeechBubbleSO : ScriptableObject
{

    [Tooltip("Array of each line of subtitles. In the future, you can create a method to read from a text file so you dont have to manual input this.")]
    public String[] subtitleText; 
    
    [Tooltip("Pauses between switching between subtitle text.")]
    public float subtitlePacing; 
    public AudioClip audioClip; 
    
}
