using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        _isRootState = true; 
        InitializeSubState();
    }

    public override void EnterState() {
        HandleJump(); 
    }

    public override void UpdateState() {

        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {
        _context.Animator.SetBool(_context.IsJumpingHash, false);
        _context.IsJumpAnimating = false; 
    }

    public override void CheckSwitchStates() {
        //Issue with the isGrounded from Character Controller. Using Raycast instead

        //if (_context.CharacterController.isGrounded)
        //{
        //    SwitchState(_factory.Grounded()); 
        //}
        Debug.Log(_context.CharacterController.isGrounded); 
        if(_context.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded()); 
        }
    }

    public override void InitializeSubState()
    {
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

    void HandleJump()
    {
        //Jump animation
        _context.Animator.SetBool(_context.IsJumpingHash, true);
        _context.IsJumpAnimating = true;

        _context.IsJumping = true;
        _context.CurrentMovementY = _context.InitialJumpVelocity;
        _context.AppliedMovementY = _context.InitialJumpVelocity;
    }

    void HandleGravity()
    {
        bool isFalling = _context.CurrentMovementY <= 0f || !_context.IsJumpPressed;
        float fallMultiplyer = 2f;

        if (isFalling)
        {
            float previousYVelocity = _context.CurrentMovementY;
            _context.CurrentMovementY = _context.CurrentMovementY + (_context.Gravity * fallMultiplyer * Time.deltaTime);
            _context.AppliedMovementY = (previousYVelocity + _context.CurrentMovementY) * .5f;

        }
        else
        {
            float previousYVelocity = _context.CurrentMovementY;
            _context.CurrentMovementY = _context.CurrentMovementY + (_context.Gravity * Time.deltaTime);
            _context.AppliedMovementY = (previousYVelocity + _context.CurrentMovementY) * .5f;

        }
    }
}
