using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundComboState : MeleeBaseState
{
    public override void OnEnter(StateMachine stateMachine)
    {
        base.OnEnter(stateMachine);
        
        attackIndex = 2;
        duration = 0.5f;
        animator.SetTrigger("Attack"+attackIndex);
        Debug.Log("Attack"+attackIndex);
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (time < duration)
            return;
        
        if (shouldCombo)
        {
            stateMachine.SetNextState(new GroundFinisherState());
        }
        else
        {
            stateMachine.SetNextStateToMain();
        }
    }
}
