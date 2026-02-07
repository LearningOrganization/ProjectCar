using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float Sensitivity = 100;
    public float TopClamp = 80;
    public float BottomClamp = -80;
    public float Radius;

    [SerializeField] private Transform _targetObject;

    private Vector2 _rotation = Vector2.zero;
    private PlayerInputControls _playerInputControls;
    private Camera _playerCamera;
    private bool _isInitialized = false;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        _playerInputControls = PlayerManager.Instance.Input;
        // should be replaced by DI
        _playerCamera = GetComponentInChildren<Camera>();
        if (_targetObject != null)
        {
            InitializeCameraPosition();
        }
    }

    private void InitializeCameraPosition()
    {

        Vector3 offset = _targetObject.rotation * new Vector3(0, 0, -Radius);
        
        transform.position = _targetObject.position + offset;
        transform.LookAt(_targetObject);
        
        Vector3 angles = transform.eulerAngles;
        _rotation.x = angles.y;
    }

    // should be adapted for keyboard 
    private void LateUpdate()
    {
        if (!_isInitialized)
        {
            if (PlayerManager.Instance != null && PlayerManager.Instance.Input != null)
            {
                _playerInputControls = PlayerManager.Instance.Input;
                _isInitialized = true;
            }
            else
            {
                return;
            }
        }

        if (_targetObject == null || _playerInputControls == null)
            return;

        // camera rotation for gamepad 
        Vector2 inputVector = _playerInputControls.PlayerInput.Look;

        _rotation.x += inputVector.x * Sensitivity * Time.deltaTime;
        _rotation.y += inputVector.y * Sensitivity * Time.deltaTime;
        _rotation.y = Mathf.Clamp(_rotation.y, BottomClamp, TopClamp);

        Quaternion rotationQuat = Quaternion.Euler(_rotation.y, _rotation.x, 0);
        Vector3 offset = rotationQuat * new Vector3(0, 0, -Radius);

        this.transform.position = _targetObject.position + offset;
        this.transform.LookAt(_targetObject);

    }

}





