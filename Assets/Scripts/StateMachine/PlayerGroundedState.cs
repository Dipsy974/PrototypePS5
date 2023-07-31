using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext,playerStateFactory) {
        _isRootState = true; 
        InitializeSubState();
    }
    public override void EnterState() {
        _context.CurrentMovementY = _context.Gravity;
        _context.AppliedMovementY = _context.Gravity;
        Debug.Log("Entering Grounded");
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates() {
        //if player is grounded and jump, switch to jump state
        if (_context.IsJumpPressed)
        {
            SwitchState(_factory.Jump()); 
        }else if (!_context.CharacterController.isGrounded)
        {
            SwitchState(_factory.Fall());
        }

    }

    public override void InitializeSubState() {
        if(!_context.IsMovementPressed && !_context.IsRunPressed)
        {
            SetSubState(_factory.Idle());
        }else if(_context.IsMovementPressed && !_context.IsRunPressed)
        {
            SetSubState(_factory.Walk());
        }
        else if(_context.IsRollPressed && !_context.IsRunPressed)
        {
            SetSubState(_factory.Roll());
        }
        else
        {
            SetSubState(_factory.Run()); 
        }
    }

    
}
