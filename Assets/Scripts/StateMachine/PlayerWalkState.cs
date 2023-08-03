using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        Debug.Log("Entering Walk");
        _context.Animator.SetBool(_context.IsWalkingHash, true);
        _context.Animator.SetBool(_context.IsRunningHash, false);
        _context.Animator.SetBool(_context.IsRollingHash, false);
    }

    public override void UpdateState() {

        _context.AppliedMovementX = _context.CurrentMovementInput.x * _context.movementSpeed;
        _context.AppliedMovementZ = _context.CurrentMovementInput.y * _context.movementSpeed;
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates() {

        if (!_context.IsMovementPressed)
        {
            SwitchState(_factory.Idle());
        }else if (_context.IsMovementPressed && _context.IsRunPressed)
        {
            SwitchState(_factory.Run());
        }
        else if (_context.IsRollPressed && _context.CharacterController.isGrounded && _context.CanRoll)
        {
            SwitchState(_factory.Roll()); 
        }
    }

    public override void InitializeSubState() { }
}
