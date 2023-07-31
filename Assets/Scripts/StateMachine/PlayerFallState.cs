using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        _isRootState = true;
        InitializeSubState();
    }

    public override void EnterState()
    {
        Debug.Log("Entering Fall");
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        if (_context.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }
    }

    public override void InitializeSubState() {
        if (!_context.IsMovementPressed && !_context.IsRunPressed)
        {
            SetSubState(_factory.Idle());
        }
        else if (_context.IsMovementPressed && !_context.IsRunPressed)
        {
            SetSubState(_factory.Walk());
        }
        else
        {
            SetSubState(_factory.Run());
        }
    }

    void HandleGravity()
    {
        float previousYVelocity = _context.CurrentMovementY;
        _context.CurrentMovementY = _context.CurrentMovementY + (_context.Gravity * Time.deltaTime);
        _context.AppliedMovementY = (previousYVelocity + _context.CurrentMovementY) * .5f;
    }
}
