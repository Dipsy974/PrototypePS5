using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public GameObject basicCamera;
    public GameObject focusCamera; 
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchCameraStyle(); 
        }
    }

    private void SwitchCameraStyle()
    {
        if (basicCamera.activeInHierarchy)
        {
            basicCamera.SetActive(false);
            focusCamera.SetActive(true);
        }
        else
        {
            basicCamera.SetActive(true);
            focusCamera.SetActive(false);
        }
    }
}
