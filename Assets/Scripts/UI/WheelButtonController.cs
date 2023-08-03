
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelButtonController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private string itemName;

    private bool _isSelected;

    // Update is called once per frame
    void Update()
    {
        if (_isSelected)
        {
            itemText.text = itemName;
        }
    }

    public void OnSelected()
    {
        _isSelected = true;
    }

    public void OnUnselected()
    {
        _isSelected = false;
    }

    public void OnHoverEnter()
    {
        itemText.text = itemName;
    }
    
    public void OnHoverExit()
    {
        itemText.text = "";
    }
}
