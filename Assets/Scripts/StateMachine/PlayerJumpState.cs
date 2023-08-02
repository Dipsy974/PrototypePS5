using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        _isRootState = true; 
        InitializeSubState();
    }

    public override void EnterState() {
        HandleJump(); 
        Debug.Log("Entering Jump");
        
    }

    public override void UpdateState() {

        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {
        _context.Animator.SetBool(_context.IsJumpingHash, false);
        _context.IsJumpAnimating = false;
        _context.StartCoroutine(WaitUntilJumpPressedAgain());
        _context.WasJumping = true;
    }

    public override void CheckSwitchStates() {

        if(_context.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded()); 
        }
        else if (_context.CurrentMovementY <= 0f || !_context.IsJumpPressed)
        {
            SwitchState(_factory.Fall());
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
        float previousYVelocity = _context.CurrentMovementY;
        _context.CurrentMovementY = _context.CurrentMovementY + (_context.Gravity * Time.deltaTime);
        _context.AppliedMovementY = (previousYVelocity + _context.CurrentMovementY) * .5f;
    }
    
    private IEnumerator WaitUntilJumpPressedAgain()
    {
        _context.CanJump = false;
        yield return new WaitUntil(()=> !_context.IsJumpPressed);
        _context.CanJump = true;
    }
}
