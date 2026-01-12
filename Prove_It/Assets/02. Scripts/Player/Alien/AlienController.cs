using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlienController : MonoBehaviour
{
    public event Action InteractPressed;

    private Rigidbody rigid;
    private Animator animator;
    private Transform trans;
    private Vector3 defaultScale;
    private Vector3 penaltyScale;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private TaskUI taskUI;
    [SerializeField] private InteractUI interactUI;

    private Vector2 input;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        trans = GetComponent<Transform>();
        defaultScale = trans.localScale;
        penaltyScale = defaultScale + new Vector3(0.2f, 0.2f, 0.2f);
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

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (interactUI.isConversation) return;
 
        var v = rigid.velocity;
        v.y = 2f;              
        rigid.velocity = v;

        taskUI.ReduceGauge();
    }

    public void Penalty()
    {
        trans.localScale = penaltyScale;
    }

    public void X()
    {
        trans.localScale = defaultScale;
    }
}
