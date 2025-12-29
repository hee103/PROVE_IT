using UnityEngine;
using UnityEngine.InputSystem;

public class InspectorController : MonoBehaviour
{
    [Header("Look")]
    private Vector2 lookInput;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float sensitivity;
    [SerializeField] private float xMin = -75;
    [SerializeField] private float xMax = 75;
    [SerializeField] private bool cursurLockState;
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
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void Look()
    {
        curCamX += lookInput.y * sensitivity;
        curCamX = Mathf.Clamp(curCamX, xMin, xMax);

        cameraPivot.localEulerAngles = new Vector3(-curCamX, 0f, 0f);
        transform.eulerAngles += new Vector3(0, lookInput.x * sensitivity, 0);
    }
}
