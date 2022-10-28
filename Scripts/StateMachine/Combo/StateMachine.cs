using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public string customName;

    private State mainStateType;

    public State currentState { get; private set; }
    private State nextState;

    // Update is called once per frame
    void Update()
    {
        if (nextState != null)
        {
            SetState(nextState);
        }

        if (currentState != null)
            currentState.OnUpdate();
    }

    private void SetState(State newState)
    {
        nextState = null;
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void SetNextState(State newState)
    {
        if (newState != null)
        {
            nextState = newState;
        }
    }

    private void LateUpdate()
    {
        if (currentState != null)
            currentState.OnLateUpdate();
    }

    private void FixedUpdate()
    {
        if (currentState != null)
            currentState.OnFixedUpdate();
    }

    public void SetNextStateToMain()
    {
        nextState = mainStateType;
        InputManager.instance.canBlock = true;
    }

    private void Awake()
    {
        SetNextStateToMain();

    }


    private void OnValidate()
    {
        if (mainStateType != null)
            return;
        
        if (customName != "Combat")
            return;
        
        mainStateType = new IdleCombatState();
        
        
    }
}
