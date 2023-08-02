using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollingState : PlayerBaseState
{
    
    public PlayerRollingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory): base(currentContext,playerStateFactory) {
    }
    
    public override void EnterState()
    {
        PrepareRoll();
        HandleRoll();
    }

    public override void UpdateState() {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _context.StartCoroutine(TriggerRollCooldown());
        _context.IsRolling = false;
        Debug.Log("Exiting Roll");
    }

    public override void CheckSwitchStates() {
        if (_context.RollDone)
        {
            if (!_context.IsMovementPressed)
            {
                SwitchState(_factory.Idle());
            }else if (_context.IsMovementPressed && _context.IsRunPressed)
            {
                SwitchState(_factory.Run());
            }else if (_context.IsMovementPressed)
            {
                SwitchState(_factory.Walk());
            }
        }
        
    }

    public override void InitializeSubState() { }

    private void HandleRoll()
    {
        _context.Animator.SetBool(_context.IsRollingHash, true);
        
        _context.AppliedMovementX = _context.RollDirection.x * _context.RollSpeed;
        _context.AppliedMovementZ = _context.RollDirection.z * _context.RollSpeed;
    }

    private void PrepareRoll()
    {
        _context.Animator.SetBool(_context.IsWalkingHash, false);
        _context.Animator.SetBool(_context.IsRunningHash, false); 
        _context.AppliedMovementX = 0f;
        _context.AppliedMovementZ = 0f;
        _context.RollDone = false;
        _context.StartCoroutine(ExitRoll());
        _context.IsRolling = true;
        _context.RollDirection = _context.transform.forward;
    }
    
    private IEnumerator ExitRoll()
    {
        yield return new WaitForSeconds(_context.RollTime);
        _context.RollDone = true;
    }
    
    private IEnumerator TriggerRollCooldown()
    {
        _context.CanRoll = false;
        yield return new WaitForSeconds(_context.RollCooldown);
        yield return new WaitUntil(()=> !_context.IsRollPressed);
        _context.CanRoll = true;
    }
    
}
