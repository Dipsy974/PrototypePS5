using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
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
    public  float maxJumpTime = 0.75f;
    private bool _isJumping = false;
    private bool _isJumpAnimating;
    private int _jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> jumpGrativities = new Dictionary<int, float>();


    //Gravity variables
    private float _gravity = -9.8f;
    private float _groundedGravity = -.05f; 

    //Animation Hashes
    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isJumpingHash; 
    private int _isDoubleJumpingHash; 

    private float _rotationFactorPerFrame = 15.0f;
    public float movementSpeed; 
    public float runMultiplyer; 

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>(); 

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isDoubleJumpingHash = Animator.StringToHash("isDoubleJumping");

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


        SetUpJumpVariables(); 

    }
    

    void Update()
    {
        if (!_isJumping)
        {
            HandleRotation();
        }

        HandleAnimation();

        if (_isRunPressed)
        {
            _appliedMovement.x = _currentRunMovement.x * movementSpeed;
            _appliedMovement.z = _currentRunMovement.z * movementSpeed;

        }
        else
        {
            _appliedMovement.x = _currentMovement.x * movementSpeed;
            _appliedMovement.z = _currentMovement.z * movementSpeed;
        }

        //Convert Movement relative to Camera
        _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);

    
        _characterController.Move(_cameraRelativeMovement * Time.deltaTime);
       


        HandleGravity();
        HandleJump();
    }

    private void SetUpJumpVariables()
    {
        float timeToApex = maxJumpTime / 2; 

        _gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;

        float secondJumpGravity = (-2 * maxJumpHeight) / Mathf.Pow((timeToApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * maxJumpHeight) / (timeToApex * 1.25f);

        initialJumpVelocities.Add(1, _initialJumpVelocity);
        initialJumpVelocities.Add(2, secondJumpInitialVelocity);

        jumpGrativities.Add(0, _gravity);
        jumpGrativities.Add(1, _gravity);
        jumpGrativities.Add(2, secondJumpGravity);

    }

    private void HandleJump()
    {
        if(!_isJumping && (_characterController.isGrounded || _jumpCount < 2) && _isJumpPressed)
        {
            //Jump animation
            if(_jumpCount < 1)
            {
                _animator.SetBool(_isJumpingHash, true);
            }
            else
            {
                _animator.SetBool(_isDoubleJumpingHash, true);
            }
 
            _isJumpAnimating = true; 

            _isJumping = true;

            _jumpCount++; 

            _currentMovement.y = initialJumpVelocities[_jumpCount];
            _appliedMovement.y = initialJumpVelocities[_jumpCount]; 
        }
        else if(!_isJumpPressed && _isJumping && (_characterController.isGrounded || _jumpCount < 2)) {
            _isJumping = false; 
        }
    }

    private void HandleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning = _animator.GetBool(_isRunningHash);

        //Walking handling
        if(_isMovementPressed && !isWalking)
        {
            _animator.SetBool(_isWalkingHash, true); 
        }
        else if(!_isMovementPressed && isWalking)
        {
            _animator.SetBool(_isWalkingHash, false); 
        }

        
        //Running handling
        if((_isMovementPressed && _isRunPressed) && !isRunning)
        {
            _animator.SetBool(_isRunningHash, true);
        }
        else if((!_isMovementPressed || !_isRunPressed) && isRunning)
        {
            _animator.SetBool(_isRunningHash, false);
        }

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

    private void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0f || !_isJumpPressed;
        float fallMultiplyer = 2f;


        if (_characterController.isGrounded)
        {
            //Stop jumping animation
            if (_isJumpAnimating)
            {
                _animator.SetBool(_isJumpingHash, false);
                _animator.SetBool(_isDoubleJumpingHash, false);
                _isJumpAnimating = false;
                _jumpCount = 0; 
            }

            _currentMovement.y = _groundedGravity;
            _appliedMovement.y = _groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (jumpGrativities[_jumpCount] * fallMultiplyer * Time.deltaTime);
            _appliedMovement.y = (previousYVelocity + _currentMovement.y) * .5f;

        }
        else
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (jumpGrativities[_jumpCount] * Time.deltaTime);
            _appliedMovement.y = (previousYVelocity + _currentMovement.y) * .5f; 

        }
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

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable(); 
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
