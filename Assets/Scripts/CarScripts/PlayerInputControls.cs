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

    public GearShiftCommand ShiftCommand;

}

public enum GearShiftCommand
{
    None,
    Up,
    Down
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

        GearShiftCommand shiftCommand = GearShiftCommand.None;

        if (Permissions.HasFlag(InputPermissions.GearShift))
        {
            if (_controls.Player.ShiftUp.triggered)
                shiftCommand = GearShiftCommand.Up;
            else if (_controls.Player.ShiftDown.triggered)
                shiftCommand = GearShiftCommand.Down;
        }

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

            ShiftCommand = shiftCommand
        };
    }

    public GearShiftCommand ConsumeShiftCommand()
    {
        var cmd = PlayerInput.ShiftCommand;

        if (cmd == GearShiftCommand.None)
            return cmd;

        var input = PlayerInput;
        input.ShiftCommand = GearShiftCommand.None;
        PlayerInput = input;

        return cmd;
    }
}

