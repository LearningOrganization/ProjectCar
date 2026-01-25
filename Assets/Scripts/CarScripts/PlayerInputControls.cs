using UnityEngine;

public struct PlayerInput
{
    public Vector2 Look;
    public Vector2 WheelsRotating; 
    public float Acceleration;
    public float Break;
    public bool Handbrake;

    public bool ShiftUp;
    public bool ShiftDown;

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
            WheelsRotating = _controls.Player.WheelsRotating.ReadValue<Vector2>(),

            Acceleration = _controls.Player.Acceleration.ReadValue<float>(),
            Break = _controls.Player.Break.ReadValue<float>(),
            Handbrake = _controls.Player.HandBrake.ReadValue<float>() > 0.5f,

        };
    }
}

