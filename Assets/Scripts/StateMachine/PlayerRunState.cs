using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        _context.Animator.SetBool(_context.IsRunningHash, true);
        _context.Animator.SetBool(_context.IsWalkingHash, true);
        _context.Animator.SetBool(_context.IsRollingHash, false);
    }

    public override void UpdateState() {

        _context.AppliedMovementX = _context.CurrentMovementInput.x * _context.movementSpeed * _context.runMultiplyer;
        _context.AppliedMovementZ = _context.CurrentMovementInput.y * _context.movementSpeed * _context.runMultiplyer;
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates() {

        if (!_context.IsMovementPressed)
        {
            SwitchState(_factory.Idle());
        }
        else if (_context.IsMovementPressed && !_context.IsRunPressed)
        {
            SwitchState(_factory.Walk());
        }
        else if (_context.IsRollPressed && _context.CharacterController.isGrounded && _context.CanRoll)
        {
            SwitchState(_factory.Roll()); 
        }
        else if (_context.IsAttackPressed && _context.CharacterController.isGrounded)
        {
            SwitchState(_factory.Attack());
        }
    }

    public override void InitializeSubState() { }
}
