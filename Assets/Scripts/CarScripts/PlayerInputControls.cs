using System;
using Unity.Splines.Examples;
using UnityEngine;

public struct PlayerInput
{
    public Vector2 Look;
    public Vector2 WheelsRotatingInput; 
    public float ThrottleInput;
    public float BrakeInput;
    public bool Handbrake;

    public bool ShiftUpRequested;
    public bool ShiftDownRequested;

}

[Flags]
public enum InputPermissions
{
    None = 0,
    Steering = 1 << 0,
    Driving = 1 << 1,
    Camera = 1 << 2,
    GearShift = 1 << 3,

    All = ~0
}

public class PlayerInputControls : MonoBehaviour
{
    public PlayerInput PlayerInput { get; private set; }
    public InputPermissions Permissions { get; private set; } = InputPermissions.All;
    InputSystem_Actions _controls;

    public void SetPermissions(InputPermissions permissions)
    {
        Permissions = permissions;
    }

    void OnDestroy()
    {
        _controls.Dispose();
    }

    void Awake()
    {
        _controls = new InputSystem_Actions();
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }

    void Update()
    {
        // not the best solution, but it works at now
        PlayerInput = new PlayerInput
        {
            Look = Permissions.HasFlag(InputPermissions.Camera)
                ? _controls.Player.Look.ReadValue<Vector2>()
                : Vector2.zero,

            WheelsRotatingInput = Permissions.HasFlag(InputPermissions.Steering)
                ? _controls.Player.WheelsRotating.ReadValue<Vector2>()
                : Vector2.zero,

            ThrottleInput = Permissions.HasFlag(InputPermissions.Driving)
                ? _controls.Player.ThrottleInput.ReadValue<float>()
                : 0f,

            BrakeInput = Permissions.HasFlag(InputPermissions.Driving)
                ? _controls.Player.Break.ReadValue<float>()
                : 0f,

            Handbrake = Permissions.HasFlag(InputPermissions.Driving)
                && _controls.Player.HandBrake.ReadValue<float>() > 0.5f,

            ShiftUpRequested = Permissions.HasFlag(InputPermissions.GearShift)
                && _controls.Player.ShiftUp.triggered,

            ShiftDownRequested = Permissions.HasFlag(InputPermissions.GearShift)
                && _controls.Player.ShiftDown.triggered
        };
    }

    public void ConsumeShiftInputs()
    {
        var input = PlayerInput;

        input.ShiftUpRequested = false;
        input.ShiftDownRequested = false;

        PlayerInput = input;
    }
}

