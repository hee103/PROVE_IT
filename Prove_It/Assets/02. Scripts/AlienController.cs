using UnityEngine;
using UnityEngine.InputSystem;

public class AlienController : MonoBehaviour
{
    private Rigidbody rigid;

    [SerializeField] private float moveSpeed = 3f;

    private Vector2 input;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        AlienMove();
    }

    private void AlienMove()
    {
        Vector3 dir = transform.forward * input.y + transform.right * input.x;
        Vector3 vel = new Vector3(dir.x * moveSpeed, rigid.velocity.y, dir.z * moveSpeed);
        rigid.velocity = vel;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            input = context.ReadValue<Vector2>();
        else if (context.canceled)
            input = Vector2.zero;
    }
}
