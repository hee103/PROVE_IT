using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody rb;

    private Vector2 moveInput;

    private void Awake()
    {

        //카메라 위치 보간
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x);
        Vector3 velocity = moveDir * moveSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }
}
