using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public PlayerInputActions playerInputActions;
    [HideInInspector] public InputAction jump;
    [HideInInspector] public InputAction move;
    [HideInInspector] public InputAction attack;
    [HideInInspector] public InputAction block;
    [HideInInspector] public InputAction dash;
    [HideInInspector] public InputAction burst;
    [HideInInspector] public InputAction pause;


    [HideInInspector] public InputAction submit;
    [HideInInspector] public InputAction cancel;
    [HideInInspector] public InputAction navigate;
    [HideInInspector] public InputAction click;
    [HideInInspector] public InputAction middleClick;
    [HideInInspector] public InputAction rightClick;
    [HideInInspector] public InputAction unpause;


    [HideInInspector] public bool canBlock;
    [HideInInspector] public bool dashBlockAttack;


    private void Awake()
    {
        instance = this;
        playerInputActions = new PlayerInputActions();
        InitializingInputs();
        OnEnableInput();
        
    }

    private void Update()
    {
        
    }

    private void InitializingInputs()
    {
        jump = playerInputActions.Player.Jump;
        move = playerInputActions.Player.HorizontalMovement;
        attack = playerInputActions.Player.Attack;
        block = playerInputActions.Player.Block;
        dash = playerInputActions.Player.Dash;
        burst = playerInputActions.Player.Burst;
        pause = playerInputActions.Player.Pause;
        submit = playerInputActions.UI.Submit;
        cancel = playerInputActions.UI.Cancel;
        navigate = playerInputActions.UI.Navigate;
        click = playerInputActions.UI.Click;
        middleClick = playerInputActions.UI.MiddleClick;
        rightClick = playerInputActions.UI.RightClick;
        unpause = playerInputActions.UI.Unpause;
    }
    public void OnEnableInput()
    {
        jump.Enable();
        move.Enable();
        attack.Enable();
        block.Enable();
        dash.Enable();
        burst.Enable();
        pause.Enable();
    }

    public void OnDisableInput()
    {
        jump.Disable();
        move.Disable();
        attack.Disable();
        block.Disable();
        dash.Disable();
        burst.Disable();
        pause.Disable();
    }

    public void OnEnableUIInput()
    {
        submit.Enable();
        cancel.Enable();
        navigate.Enable();
        click.Enable();
        middleClick.Enable();
        rightClick.Enable();
        unpause.Enable();
    }

    public void OnDisableUIInput()
    {
        submit.Disable();
        cancel.Disable();
        navigate.Disable();
        click.Disable();
        middleClick.Disable();
        rightClick.Disable();
        unpause.Disable();
    }
    
    
}