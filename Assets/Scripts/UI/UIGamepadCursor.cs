using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;


public class UIGamepadCursor : MonoBehaviour
{
    [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;
    [SerializeField] private RectTransform _cursorRectTransform;
    [SerializeField] private RectTransform _canvasRectTransform;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private float _cursorSpeed = 1000f;

    private bool _previousMouseState;
    private Mouse _virtualMouse;
    private Mouse _currentMouse;
    private Camera _mainCamera;

    private string _previousControlScheme = "";
    private const string _gamepadScheme = "Gamepad";
    private const string _mouseScheme = "KeyboardMouse";

    private void OnEnable()
    {
        
        _currentMouse = Mouse.current;
        _mainCamera = Camera.main;
        
        if (_virtualMouse == null)
        {
            _virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!_virtualMouse.added)
        {
            InputSystem.AddDevice(_virtualMouse);
        }

        InputUser.PerformPairingWithDevice(_virtualMouse, _playerInput.user);

        if (_cursorRectTransform != null)
        {
            Vector2 position = _cursorRectTransform.anchoredPosition;
            InputState.Change(_virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        _playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateMotion;
        _playerInput.onControlsChanged -= OnControlsChanged;
        if(_virtualMouse != null && _virtualMouse.added) InputSystem.RemoveDevice(_virtualMouse);
    }

    private void UpdateMotion()
    {
        if (_virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();
        deltaValue *= _cursorSpeed * Time.unscaledTime;

        Vector2 currentPosition = _virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);
        
        InputState.Change(_virtualMouse.position, newPosition);
        InputState.Change(_virtualMouse.delta, deltaValue);
        
        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, position, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null :_mainCamera, out anchoredPosition); //if render mode of canvas is in screenSpaceOverlay, no need for camera reference
        _cursorRectTransform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChanged(UnityEngine.InputSystem.PlayerInput input)
    {
        if (_playerInput.currentControlScheme == _mouseScheme && _previousControlScheme != _mouseScheme)
        {
            InputSystem.onAfterUpdate -= UpdateMotion;
            _cursorRectTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            _currentMouse.WarpCursorPosition(_virtualMouse.position.ReadValue());
            _previousControlScheme = _mouseScheme;
        }
        else if (_playerInput.currentControlScheme == _gamepadScheme && _previousControlScheme != _gamepadScheme)
        {
            InputSystem.onAfterUpdate += UpdateMotion;
            _cursorRectTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(_virtualMouse.position, _currentMouse.position.ReadValue());
            AnchorCursor(_currentMouse.position.ReadValue());
            _previousControlScheme = _gamepadScheme;
        }
    }
}
