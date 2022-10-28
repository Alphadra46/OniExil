using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class Initializer : MonoBehaviour
{
    public static SaveVolume saveVolume;
    private string path;
    public Save dataScriptableObject;
    public AudioMixer audioManager;


    private void Awake()
    {
        path = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        
        if (!dataScriptableObject.firstLaunch)
            return;

        if (File.Exists(path+"SaveVolumeData"))
        {
            Debug.Log(path);
            StreamReader reader = new StreamReader(path + "SaveVolumeData");
            string json = reader.ReadToEnd();
            saveVolume = JsonUtility.FromJson<SaveVolume>(json);
            dataScriptableObject.saveVolume = saveVolume;
        }
        else
        {
            SaveVolume created = new SaveVolume{masterVolume = 100, musicVolume = 100,soundEffectsVolume = 100};
            string json = JsonUtility.ToJson(created);
            Debug.Log(json);
            StreamWriter writer = new StreamWriter(path + "SaveVolumeData");
            writer.Write(json);
            writer.Close();
            Debug.Log(json);
            dataScriptableObject.saveVolume = created;
        }
        
    }

    private void Start()
    {
        audioManager.SetFloat("MasterVolume", Mathf.Log10(dataScriptableObject.saveVolume.masterVolume / 100) * 20);
        audioManager.SetFloat("MusicVolume", Mathf.Log10(dataScriptableObject.saveVolume.musicVolume / 100) * 20);
        audioManager.SetFloat("SFXVolume", Mathf.Log10(dataScriptableObject.saveVolume.soundEffectsVolume / 100) * 20);
    }

    private void OnApplicationQuit()
    {
        saveVolume = dataScriptableObject.saveVolume;
        string json = JsonUtility.ToJson(saveVolume);
        StreamWriter writer = new StreamWriter(path + "SaveVolumeData");
        writer.Write(json);
        writer.Close();
    }
}
