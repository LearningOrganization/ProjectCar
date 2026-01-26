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

public class PlayerInputControls : MonoBehaviour
{
    public PlayerInput PlayerInput { get; private set; }
    InputSystem_Actions _controls;

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
        PlayerInput = new PlayerInput
        {
            Look = _controls.Player.Look.ReadValue<Vector2>(),
            WheelsRotatingInput = _controls.Player.WheelsRotating.ReadValue<Vector2>(),

            ThrottleInput = _controls.Player.ThrottleInput.ReadValue<float>(),
            BrakeInput = _controls.Player.Break.ReadValue<float>(),
            Handbrake = _controls.Player.HandBrake.ReadValue<float>() > 0.5f,

            ShiftUpRequested = _controls.Player.ShiftUp.triggered,
            ShiftDownRequested = _controls.Player.ShiftDown.triggered
        };
    }
}

