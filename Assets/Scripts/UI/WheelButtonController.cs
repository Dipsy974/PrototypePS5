
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelButtonController : MonoBehaviour
{
    public int id;
    public string itemName;
    
    [SerializeField] private SelectionWheel _wheel;
    private Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
    }

    public void OnHoverEnter()
    {
        if (_button != null && _button.interactable)
        {
            _wheel.SelectNewButton(this);
        }
    }
    
    public void OnHoverExit()
    {
        _wheel.UnselectButton();
    }
}
