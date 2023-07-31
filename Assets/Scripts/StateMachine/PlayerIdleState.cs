using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    public override void EnterState() {
        _context.Animator.SetBool(_context.IsWalkingHash, false);
        _context.Animator.SetBool(_context.IsRunningHash, false);
        _context.AppliedMovementX = 0;
        _context.AppliedMovementZ = 0;
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates() {
    
        if(_context.IsMovementPressed && _context.IsRunPressed)
        {
            SwitchState(_factory.Run());
        }else if (_context.IsMovementPressed)
        {
            SwitchState(_factory.Walk()); 
        }
        else if (_context.IsRollPressed)
        {
            SwitchState(_factory.Roll()); 
        }
    }

    public override void InitializeSubState() { }
}
