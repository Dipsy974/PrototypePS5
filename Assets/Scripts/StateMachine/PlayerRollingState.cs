using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollingState : PlayerBaseState
{
    public PlayerRollingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext,playerStateFactory) {
    }
    
    public override void EnterState()
    {
        _context.Animator.SetBool(_context.IsRollingHash, true);
        Debug.Log("Entering Roll");
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _context.Animator.SetBool(_context.IsRollingHash, false);
    }

    public override void CheckSwitchStates() {
        
    }

    public override void InitializeSubState() { }
}
