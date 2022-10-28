using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFinisherState : MeleeBaseState
{
    public override void OnEnter(StateMachine stateMachine)
    {
        base.OnEnter(stateMachine);
        
        attackIndex = 3;
        duration = 0.75f;
        animator.SetTrigger("Attack"+attackIndex);
        Debug.Log("Attack"+attackIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (time < duration)
            return;
        
        stateMachine.SetNextStateToMain();
        
    }
}
