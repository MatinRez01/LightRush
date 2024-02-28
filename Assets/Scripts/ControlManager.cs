using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControlManager : MonoBehaviour
{
    PlayerInputManager playerInput;
    [SerializeField]GameObject joystick;
    public UnityEvent OnJoystickEnable;
    [SerializeField]GameObject Button;
    public UnityEvent OnButtonEnable;


    private void OnEnable()
    {
        playerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInputManager>();
        if(!PlayerPrefs.HasKey("Control"))
        {
            PlayerPrefs.SetString("Control", "Button");
        }
        switch( PlayerPrefs.GetString("Control"))
        {
            case "Joystick":
                EnableJoystick();
                SwitchControlButtons(false);
                break;
            case "Button":
                EnableButtons();
                SwitchControlButtons(true);
                break;
        }
    }
    private void OnDisable()
    {
        Handheld.Vibrate();
    }
    public void EnableJoystick()
    {
        playerInput.inputType = PlayerInputManager.InputType.Joystick;
        PlayerPrefs.SetString("Control", "Joystick");
        SwitchControlButtons(false);
        OnJoystickEnable.Invoke();


    }
    public void EnableButtons()
    {
        playerInput.inputType = PlayerInputManager.InputType.Button;
        PlayerPrefs.SetString("Control", "Button");
        SwitchControlButtons(true);
        OnButtonEnable.Invoke();

    }
    public void SwitchControlButtons(bool buttonEnabled)
    {
        joystick.SetActive(!buttonEnabled);
        Button.SetActive(buttonEnabled);
    }
}
