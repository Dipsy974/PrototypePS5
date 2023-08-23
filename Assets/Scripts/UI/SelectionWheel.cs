using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionWheel : MonoBehaviour
{
    [SerializeField] private List<WheelButtonController> _buttons;
    [SerializeField] private TextMeshProUGUI _centerText;

    private WheelButtonController _currentSelectedButton;
    private WheelButtonController _lastSelectedButton;


    private Vector2 _previousMousePosition;
    private void OnEnable()
    {
        if (_currentSelectedButton == null)
        {
            SelectNewButton(_buttons[0]);
        }
        else
        {
            SelectNewButton(_currentSelectedButton);
        }
    }

    private void OnDisable()
    {
        _previousMousePosition = Mouse.current.position.ReadValue();
    }

    public void SelectNewButton(WheelButtonController newButton)
    {
        StartCoroutine(SetSelectedAfterOneFrame(newButton));
    }

    public void UnselectButton()
    {

    }

    public void UnlockNewAbility(int id)
    { 
        Button button = _buttons[id].GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
            button.transform.GetChild(0).gameObject.SetActive(true);
        } 
    }
    

    private IEnumerator SetSelectedAfterOneFrame(WheelButtonController newButton)
    {
        yield return null;
        if (_currentSelectedButton != newButton)
        {
            EventSystem.current.SetSelectedGameObject(newButton.gameObject);
            _currentSelectedButton = newButton;
            _lastSelectedButton = newButton;
        }
    }
    
    public void Update()
    {
        if (_currentSelectedButton != null)
        {
            _centerText.text = _currentSelectedButton.itemName;
        }
    }
}
