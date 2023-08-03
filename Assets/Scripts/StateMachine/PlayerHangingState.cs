using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangingState : PlayerBaseState
{
 public PlayerHangingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        _isRootState = true; 
        InitializeSubState();
    }

    public override void EnterState() {
        _context.Animator.SetBool(_context.IsHangingHash, true);
        _context.IsHanging = true;
        
        //Disabling character controller colliders to properly grab ledge, could maybe end up causing problems
        _context.CharacterController.enabled = false;
        SnapToHangingPoint();
        Debug.Log("Entering Hang");
    }

    public override void UpdateState() {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {
        _context.Animator.SetBool(_context.IsHangingHash, false);
        _context.IsHanging = false;
        _context.CharacterController.enabled = true;
        _context.StartCoroutine(TriggerLedgeGrabCooldown());
    }

    public override void CheckSwitchStates() {

        if (_context.IsJumpPressed)
        {
            SwitchState(_factory.Jump());
        }
        else if (_context.IsDownLedgePressed)
        {
            SwitchState(_factory.Fall());
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
        _context.AppliedMovementY = 0f;
    }

    void SnapToHangingPoint()
    {
        Transform playerTransform = _context.transform;
        Vector3 hangPosition = _context.PositionToGrab;
        Vector3 offset = playerTransform.forward * -0.01f + playerTransform.up * -1.35f; //offset adjusted to character dimensions
        hangPosition += offset;
        playerTransform.position = hangPosition;
        playerTransform.forward = _context.DirectionToFace;
    }
    
    private IEnumerator TriggerLedgeGrabCooldown()
    {
        _context.CanGrabLedge = false;
        yield return new WaitForSeconds(0.2f);
        _context.CanGrabLedge = true;
    }
}
