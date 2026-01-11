using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoleBasedActivator : NetworkBehaviour
{
    [Header("Insepctor Only")]
    [SerializeField] private Camera inspectorCamera;
    [SerializeField] private PlayerInput inspectorInput;
    [SerializeField] private GameObject inspectorUI;

    [Header("Alien Only")]
    [SerializeField] private Camera alienCamera;
    [SerializeField] private PlayerInput alienInput;
    [SerializeField] private GameObject alienUI;

    private AssignedRole _applied = AssignedRole.None;
    private NetworkPlayer _player;

    public override void Spawned()
    {
        _player = GetComponent<NetworkPlayer>();
        SetAll(false);

        if (!Object.HasInputAuthority) return;

        ApplyIfReady();
    }

    public override void FixedUpdateNetwork()
    {
        if(!Object.HasInputAuthority) return;
        ApplyIfReady();
    }

    private void ApplyIfReady()
    {
        if (_player == null) return;
        var role = _player.Role;
        if (role == AssignedRole.None) return;
        if (_applied == role) return;

        SetAll(false);

        if(role == AssignedRole.Inspector)
        {
            if (inspectorCamera) inspectorCamera.enabled = true;
            if (inspectorInput) inspectorInput.enabled = true;
            if (inspectorUI) inspectorUI.SetActive(true);
        }
        else if(role == AssignedRole.Alien)
        {
            if (alienCamera) alienCamera.enabled = true;
            if (alienInput) alienInput.enabled = true;
            if (alienUI) alienUI.SetActive(true);
        }

        _applied = role;
    }

    private void SetAll(bool on)
    {
        if (inspectorCamera) inspectorCamera.enabled = on;
        if (alienCamera) alienCamera.enabled = on;

        if(inspectorInput) inspectorInput.enabled = on;
        if(alienInput) alienInput.enabled = on;

        if(inspectorUI) inspectorUI.SetActive(on);
        if (alienUI) alienUI.SetActive(on);
    }
}
