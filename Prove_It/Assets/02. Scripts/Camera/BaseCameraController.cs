using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCameraController : MonoBehaviour
{
    [Header("Look")]
    private Vector2 lookInput;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float sensitivity;
    [SerializeField] private float xMin = -75;
    [SerializeField] private float xMax = 75;
    [SerializeField] private bool cursurLockState;

    private float curCamX;
    private float yaw;

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

        yaw += lookInput.x * sensitivity;

        if (rb != null)
            rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }
}
