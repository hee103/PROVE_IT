using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerInputData : INetworkInput
{
    public Vector3 Move;
}

public class PlayerMovement : NetworkBehaviour
{
    [Header("설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody rb;

    private Vector2 moveInput;

    public override void Spawned()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (!Object.HasStateAuthority && rb != null)
            rb.isKinematic = true;
        else if (rb != null)
            rb.isKinematic = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (rb == null) return;

        if (GetInput<PlayerInputData>(out var input))
        {
            var f = rb.transform.forward;
            var r = rb.transform.right;

            var dir = f * input.Move.y + r * input.Move.x;
            var vel = dir * moveSpeed;
            vel.y = rb.velocity.y;
            rb.velocity = vel;
        }
    }
}
