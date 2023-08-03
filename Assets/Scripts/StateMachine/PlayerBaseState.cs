
public abstract class PlayerBaseState
{
    protected bool _isRootState = false; 
    protected PlayerStateMachine _context; 
    protected PlayerStateFactory _factory;
    protected PlayerBaseState _currentSuperState;
    protected PlayerBaseState _currentSubState;
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) {

        _context = currentContext;
        _factory = playerStateFactory; 
    }
    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates() {
        UpdateState();
        if(_currentSubState != null)
        {
            _currentSubState.UpdateStates(); 
        }
    }
    protected void SwitchState(PlayerBaseState newState) {

        ExitState();

        newState.EnterState();

        if (_isRootState)
        {
            _context.CurrentState = newState;
        }else if(_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState); 
        }
        
    }
    protected void SetSuperState(PlayerBaseState newSuperState) {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState) {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this); 
        newSubState.EnterState();
    }

}
