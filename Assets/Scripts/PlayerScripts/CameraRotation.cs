using System;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float Sensitivity = 100;
    public float TopClamp = 80;
    public float BottomClamp = -80;
    public float Radius;

    public Transform TargetObject;

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
        if (TargetObject != null)
        {
            InitializeCameraPosition();
        }
    }

    private void InitializeCameraPosition()
    {

        Vector3 offset = TargetObject.rotation * new Vector3(0, 0, -Radius);
        
        transform.position = TargetObject.position + offset;
        transform.LookAt(TargetObject);
        
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

        if (TargetObject == null || _playerInputControls == null)
            return;

        // camera rotation for gamepad 
        Vector2 inputVector = _playerInputControls.PlayerInput.Look;

        _rotation.x += inputVector.x * Sensitivity * Time.deltaTime;
        _rotation.y += inputVector.y * Sensitivity * Time.deltaTime;
        _rotation.y = Mathf.Clamp(_rotation.y, BottomClamp, TopClamp);

        Quaternion rotationQuat = Quaternion.Euler(_rotation.y, _rotation.x, 0);
        Vector3 offset = rotationQuat * new Vector3(0, 0, -Radius);

        this.transform.position = TargetObject.position + offset;
        this.transform.LookAt(TargetObject);

    }

}





