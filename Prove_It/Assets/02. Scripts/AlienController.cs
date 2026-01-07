using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlienController : MonoBehaviour
{
    public event Action InteractPressed;

    private Rigidbody rigid;
    private Animator animator;
    [SerializeField] private float moveSpeed = 3f;

    private Vector2 input;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector3 dir = transform.forward * input.y + transform.right * input.x;
        rigid.velocity = new Vector3(dir.x * moveSpeed, rigid.velocity.y, dir.z * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        input = ctx.ReadValue<Vector2>();
        animator.SetBool("isRun", input.sqrMagnitude > 0.001f);
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Debug.Log("Interact pressed");
        InteractPressed?.Invoke();
    }

}
