using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeBaseState : State
{
    public float duration;
    protected Animator animator;
    protected bool shouldCombo;
    protected int attackIndex;
    private float AttackPressedTimer = 0;
    private bool isAttacking;

    public override void OnEnter(StateMachine stateMachine)
    {
        base.OnEnter(stateMachine);
        InputManager.instance.attack.started += Attack; //TODO - Performed instead of started
        InputManager.instance.attack.canceled += CancelAttack;
        animator = GetComponent<Animator>(); //TODO - Singleton
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;
        
        
        if (isAttacking)
        {
            AttackPressedTimer = 2;
        }

        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0f)
        {
            shouldCombo = true;
            InputManager.instance.canBlock = false;
        }
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    
    private void Attack(InputAction.CallbackContext context)
    {
        if (InputManager.instance.dashBlockAttack)
            return;
        
        isAttacking = true;
    }

    private void CancelAttack(InputAction.CallbackContext context)
    {
        isAttacking = false;
    }
}
