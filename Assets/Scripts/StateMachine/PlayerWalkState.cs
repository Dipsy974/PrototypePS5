using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        _context.Animator.SetBool(_context.IsWalkingHash, true);
        _context.Animator.SetBool(_context.IsRunningHash, false);
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
    }

    public override void InitializeSubState() { }
}
