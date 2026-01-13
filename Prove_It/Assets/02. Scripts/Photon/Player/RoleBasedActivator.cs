using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoleBasedActivator : NetworkBehaviour
{
    [Header("Insepctor Only")]
    [SerializeField] private GameObject inspectorRig;

    [Header("Alien Only")]
    [SerializeField] private GameObject alienRig;

    private AssignedRole _applied = AssignedRole.None;
    private PlayerRole _role;

    private void Awake()
    {
        Debug.Log($"[RBA] Awakeon {gameObject.name}");
    }
    public override void Spawned()
    {
        Debug.Log($"[RBA] Spawned NO={(GetComponent<NetworkObject>() != null)} InputAuth={Object.HasInputAuthority} StateAuth={Object.HasStateAuthority}");
        _role = GetComponent<PlayerRole>();

        if(inspectorRig) inspectorRig.SetActive(false);
        if(alienRig) alienRig.SetActive(false);

        if (!Object.HasInputAuthority) return;

        TryApply();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        TryApply();
    }

    private void TryApply()
    {
        if (_role == null) return;

        var role = _role.Role;
        if(role == AssignedRole.None) return;
        if (_applied == role) return;

        if (inspectorRig) inspectorRig.SetActive(role == AssignedRole.Inspector);
        if (alienRig) alienRig.SetActive(role == AssignedRole.Alien);

        _applied = role;

        Debug.Log($"[RolebaseActivator] Applied role = {role} on {Object.InputAuthority}");
    }
}
