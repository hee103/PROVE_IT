using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Look")]
    private Vector2 lookInput;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float sensitivity;
    [SerializeField] private float xMin = -75;
    [SerializeField] private float xMax = 75;
    [SerializeField] private bool cursurLockState;
    [SerializeField] private Transform character;

    private float curCamX;
    private float pitch;

    private void Awake()
    {
        Cursor.lockState = cursurLockState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void LateUpdate()
    {
        Look();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is not Mouse) return;
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void Look()
    {
        curCamX += lookInput.y * sensitivity;
        curCamX = Mathf.Clamp(curCamX, xMin, xMax);

        cameraPivot.localEulerAngles = new Vector3(-curCamX, 0f, 0f);
        character.Rotate(0f, lookInput.x * sensitivity, 0f);

    }
}
