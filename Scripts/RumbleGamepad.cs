using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleGamepad : MonoBehaviour
{
    private float lowA, highA, rumbleDuration;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (Time.time > rumbleDuration)
            return;

        var gamepad = GetGamepad();
        if (gamepad == null)
            return;
        
        gamepad.SetMotorSpeeds(lowA,highA);


    }


    public void RumbleConstant(float low, float high, float duration)
    {
        lowA = low;
        highA = high;
        rumbleDuration = Time.time + duration;
        Invoke(nameof(StopRumble), duration);
    }


    public void StopRumble()
    {
        var gamepad = GetGamepad();
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }

    private Gamepad GetGamepad()
    {
        return Gamepad.all.FirstOrDefault(g => playerInput.devices.Any(d => d.deviceId == g.deviceId));
    }
    
}
