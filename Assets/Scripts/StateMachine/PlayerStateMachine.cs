using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Animator _animator;

    //Movement variables
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private Vector3 _currentRunMovement;
    private Vector3 _appliedMovement;
    private Vector3 _cameraRelativeMovement; 
    private bool _isMovementPressed;
    private bool _isRunPressed;
    
    //Jump variables
    private bool _isJumpPressed = false;
    private float _initialJumpVelocity;
    public float maxJumpHeight = 3.0f;
    public float maxJumpTime = 0.75f;
    private bool _isJumping = false;
    private bool _isJumpAnimating;
    
    //Roll variables
    private bool _isRollPressed;

    //Gravity variables
    private float _gravity = -9.8f;
    private float _groundedGravity = -.5f;

    //Animation Hashes
    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isJumpingHash;
    private int _isRollingHash;

    private float _rotationFactorPerFrame = 15.0f;
    public float movementSpeed;
    public float runMultiplyer;

    //State variables
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states; 

    //Getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    public CharacterController CharacterController { get { return _characterController; } set { _characterController = value; } }
    public Animator Animator { get { return _animator; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsRollingHash { get { return _isRollingHash; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public bool IsJumpAnimating { set { _isJumpAnimating = value; } }
    public bool IsJumping { set { _isJumping = true; } }
    public bool IsJumpPressed { get { return _isJumpPressed;  } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool IsRollPressed { get { return _isRollPressed; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }
    public float GroundedGravity { get { return _groundedGravity; } set { _groundedGravity = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }



    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        //setup State
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState(); 

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isRollingHash = Animator.StringToHash("isRolling");

        //Called when the input system first receives the input
        _playerInput.CharacterControls.Move.started += OnMovementInput;
        //Called when input is released
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        //For controller (because inbetween values between 0 and 1)
        _playerInput.CharacterControls.Move.performed += OnMovementInput;

        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;

        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        
        _playerInput.CharacterControls.Roll.started += OnRoll;
        _playerInput.CharacterControls.Roll.canceled += OnRoll;

        SetUpJumpVariables();

    }

    private void SetUpJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        _gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        
        //Convert Movement relative to Camera
        _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);

        _characterController.Move(_cameraRelativeMovement * Time.deltaTime);

        _currentState.UpdateStates(); 
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = _cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;
        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }

    }


    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;

        _currentRunMovement.x = _currentMovementInput.x * runMultiplyer;
        _currentRunMovement.z = _currentMovementInput.y * runMultiplyer;

        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }
    private void OnRoll(InputAction.CallbackContext context)
    {
        _isRollPressed = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
    
    private Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {

        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue; 
        return vectorRotatedToCameraSpace; 

    }
}
