using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data")]
public class Save : ScriptableObject
{
    public SaveVolume saveVolume;
    public bool firstLaunch;

}

[Serializable]
public struct SaveVolume
{
    public float masterVolume;
    public float musicVolume;
    public float soundEffectsVolume;
}

