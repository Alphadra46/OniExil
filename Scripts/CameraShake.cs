using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    private CinemachineVirtualCamera cmVcam;
    
    //[SerializeField] private Transform cameraFollowPoint;
    private void Awake()
    {
        instance = this;
        cmVcam = GetComponent<CinemachineVirtualCamera>();
    }


    public void ShakeCamera(float time, float intensity)
    {
        CinemachineBasicMultiChannelPerlin cmBasicMultiChannelPerlin = cmVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cmBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        
        startingIntensity = intensity;
        shakeTimer = time;
        shakeTimerTotal = shakeTimer;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cmBasicMultiChannelPerlin = cmVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cmBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1-(shakeTimer/shakeTimerTotal));
            }
        }
    }
}
