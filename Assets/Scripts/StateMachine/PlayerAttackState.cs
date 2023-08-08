using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{

    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        HandleAttack(); 
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
       
    }

    public override void CheckSwitchStates()
    {
        if (!_context.IsAttacking)
        {

            if (!_context.IsMovementPressed)
            {
                SwitchState(_factory.Idle());
            }
            else if (_context.IsMovementPressed && _context.IsRunPressed)
            {
                SwitchState(_factory.Run());
            }
            else if (_context.IsMovementPressed)
            {
                SwitchState(_factory.Walk());
            }
        }
    }

    public override void InitializeSubState() { }

    private void HandleAttack()
    {

        if (!_context.IsAttacking && _context.IsAttackPressed && !_context.IsComboFinished) //Attack only if _isComboFinished correctly reset
        {
            if (_context.AttackCount < 3 && _context.CurrentAttackResetRoutine != null)
            {
                _context.StopCoroutine(_context.CurrentAttackResetRoutine); //Stop the coroutine only if combo is not finished
            }

            _context.Animator.SetBool(_context.IsAttackingHash, true);
            _context.IsAttacking = true;
            _context.AttackCount++;
            _context.Animator.SetInteger(_context.AttackCountHash, _context.AttackCount);
        }

    }



}
