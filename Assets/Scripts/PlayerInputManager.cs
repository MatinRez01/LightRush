using System;
using System.Collections;
using System.Data;
using UnityEngine;

public class PlayerInputManager : InputManager
{
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private TwoButtonControl button;
    [SerializeField] public InputType inputType;
    [SerializeField] private float doubleTapTimeThreshold = 0.5f; // minimum time between double taps
    private float lastTapTime = 0f;
    private PlayerCar player;
    Vector2 joyDir;
    Vector2 playerDir;
    public enum InputType
    {
        Keyboard,
        Button,
        Joystick
    }
    private void Awake()
    {
#if UNITY_EDITOR_WIN
        inputType = InputType.Keyboard;
#endif
        player = GetComponent<PlayerCar>();
    }

    float GetInputDirectionFromTouch()
    {
        return button.Value;
    }

    float GetDirectionFromJoystick()
    {
        joyDir = joystick.Direction.normalized;
        playerDir = new Vector2(transform.forward.x, transform.forward.z).normalized;
        float angleJoy = Mathf.Atan2(joyDir.x, joyDir.y);
        float anglePlayer = Mathf.Atan2(playerDir.x, playerDir.y);
        float angle = Mathf.DeltaAngle(anglePlayer, angleJoy);
        if (Mathf.Abs(angle) > Mathf.PI) angle = -angle;
        angle = Mathf.Clamp(angle, -1, 1);
        return angle;
    }


    void Update()
    {
#if UNITY_EDITOR_WIN
        if (Input.GetKeyUp(KeyCode.Space))
        {
            player.SwitchLightTrailState();
        }
#endif

    }


    public override float GetSteerDirection()
    {
        
        switch (inputType)
        {
            case InputType.Keyboard:
                return Input.GetAxis("Horizontal");
            case InputType.Button:
                return GetInputDirectionFromTouch();
            case InputType.Joystick:
                return GetDirectionFromJoystick();
            default:
                return 0;
        }
    }
}
