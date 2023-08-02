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
        CheckLedgeGrab();
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
        //move values to context as serialize fields
        if (_context.WasJumping)
        {
            float fallMultiplier = 2f;
            float previousYVelocity = _context.CurrentMovementY;
            _context.CurrentMovementY = _context.CurrentMovementY + (_context.Gravity * fallMultiplier * Time.deltaTime);
            _context.AppliedMovementY = (previousYVelocity + _context.CurrentMovementY) * .5f;
        }
        else
        {
            _context.AppliedMovementY = -9.8f;
        }

    }

    private void CheckLedgeGrab()
    {
        Transform playerTransform = _context.transform;
        
        //Downward Raycast in front of player to check for ground to grab and its y position
        RaycastHit downHit;
        Vector3 lineDownStart = (playerTransform.position + Vector3.up * 1.5f) + playerTransform.forward; //float multipliers need adjustement based on character's dimensions
        Vector3 lineDownEnd = (playerTransform.position + Vector3.up * 0.7f) + playerTransform.forward;
        Physics.Linecast(lineDownStart, lineDownEnd, out downHit, LayerMask.GetMask("Ground"));
        Debug.DrawLine(lineDownStart,lineDownEnd);

        if (downHit.collider != null)
        {
            //Same but forward to get x and z of position to grab
            RaycastHit fwdHit;
            Vector3 lineFwdStart = new Vector3(playerTransform.position.x, downHit.point.y - 0.1f, playerTransform.position.z); 
            Vector3 lineFwdEnd = new Vector3(playerTransform.position.x, downHit.point.y - 0.1f, playerTransform.position.z) + playerTransform.forward;
            Physics.Linecast(lineFwdStart, lineFwdEnd, out fwdHit, LayerMask.GetMask("Ground"));
            Debug.DrawLine(lineFwdStart,lineFwdEnd);
            
            if (fwdHit.collider != null && _context.CanGrabLedge)
            {
                _context.PositionToGrab = new Vector3(fwdHit.point.x, downHit.point.y, fwdHit.point.z);
                _context.DirectionToFace = -fwdHit.normal;
                SwitchState(_factory.Hanging());
            }
        }
    }
}
