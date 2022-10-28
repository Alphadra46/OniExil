using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{

    private GameObject pauseMenu;
    private GameObject inGameUI;
    private GameObject volumeSliders;
    private GameObject controlsImage;

    private bool isInSettings = false;
    private bool isInControls = false;

    private InputDevice lastDeviceUsed;

    [SerializeField] private Save volumeData;
    [SerializeField] private AudioMixer audioMixer;

    private Slider masterSlider;
    private Slider musicSlider;
    private Slider soundEffectSlider;
    
    private void Awake()
    {
        pauseMenu = transform.GetChild(0).gameObject;
        inGameUI = transform.GetChild(1).gameObject;
        volumeSliders = pauseMenu.transform.GetChild(5).gameObject;
        controlsImage = pauseMenu.transform.GetChild(6).gameObject;
        InputManager.instance.pause.started += PauseGame;
        InputManager.instance.unpause.started += UnpauseGame;
        InputManager.instance.cancel.started += CancelSettings;
        InputManager.instance.cancel.started += CancelControls;
        InitializeVolumeSliders();
        DetectDeviceUsed();
    }

    private void InitializeVolumeSliders()
    {
        masterSlider = volumeSliders.transform.GetChild(0).GetComponent<Slider>();
        musicSlider = volumeSliders.transform.GetChild(1).GetComponent<Slider>();
        soundEffectSlider = volumeSliders.transform.GetChild(2).GetComponent<Slider>();
        masterSlider.value = volumeData.saveVolume.masterVolume;
        musicSlider.value = volumeData.saveVolume.musicVolume;
        soundEffectSlider.value = volumeData.saveVolume.soundEffectsVolume;
        MasterVolumeChange(masterSlider.value);
        MusicVolumeChange(musicSlider.value);
        SFXVolumeChange(soundEffectSlider.value);
        masterSlider.onValueChanged.AddListener(MasterVolumeChange);
        musicSlider.onValueChanged.AddListener(MusicVolumeChange);
        soundEffectSlider.onValueChanged.AddListener(SFXVolumeChange);
    }
    
    private void MasterVolumeChange(float f)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(f / 100) * 20);
        volumeData.saveVolume.masterVolume = Mathf.RoundToInt(f);
    }

    private void MusicVolumeChange(float f)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(f / 100) * 20);
        volumeData.saveVolume.musicVolume = Mathf.RoundToInt(f);
    }

    private void SFXVolumeChange(float f)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(f / 100) * 20);
        volumeData.saveVolume.soundEffectsVolume = Mathf.RoundToInt(f);
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        soundEffectSlider.onValueChanged.RemoveAllListeners();
    }


    // Update is called once per frame
    void Update()
    {
        if (!isInControls)
            return;

        if (lastDeviceUsed.displayName == "Keyboard" || lastDeviceUsed.displayName == "Mouse")
        {
            controlsImage.transform.GetChild(0).gameObject.SetActive(false);
            controlsImage.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            controlsImage.transform.GetChild(1).gameObject.SetActive(false);
            controlsImage.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void DetectDeviceUsed()
    {
        InputSystem.onActionChange += (obj, change) =>
        {
            if (change == InputActionChange.ActionPerformed)
            {
                var inputAction = (InputAction) obj;
                var lastControl = inputAction.activeControl;
                var lastDevice = lastControl.device;
                lastDeviceUsed = lastDevice;
                //Debug.Log($"device: {lastDevice.displayName}");
            }
        };
    }
    
    
    private void PauseGame(InputAction.CallbackContext context)
    {
        InputManager.instance.OnEnableUIInput();
        InputManager.instance.OnDisableInput();
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        pauseMenu.transform.GetChild(1).GetComponent<Button>().Select();
        inGameUI.SetActive(false);
    }

    private void UnpauseGame(InputAction.CallbackContext context)
    {
        Resume();
    }

    public void Resume()
    {
        InputManager.instance.OnEnableInput();
        InputManager.instance.OnDisableUIInput();
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        inGameUI.SetActive(true);
    }

    public void Settings()
    {
        volumeSliders.SetActive(true);
        volumeSliders.transform.GetChild(0).GetComponent<Slider>().Select();
        isInSettings = true;
    }

    private void CancelSettings(InputAction.CallbackContext context)
    {
        if (!isInSettings)
            return;
        
        volumeSliders.SetActive(false);
        pauseMenu.transform.GetChild(2).GetComponent<Button>().Select();
        isInSettings = false;
    }

    public void Controls()
    {
        controlsImage.SetActive(true);
        isInControls = true;
        //Show controls for the controller used
    }

    private void CancelControls(InputAction.CallbackContext context)
    {
        if (!isInControls)
            return;
        
        controlsImage.SetActive(false);
        pauseMenu.transform.GetChild(3).GetComponent<Button>().Select();
        isInControls = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
