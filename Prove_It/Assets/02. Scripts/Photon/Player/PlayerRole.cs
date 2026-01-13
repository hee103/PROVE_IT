using Fusion;
using UnityEngine;

public class PlayerRole : NetworkBehaviour
{
    [Networked] public AssignedRole Role { get; private set; }

    public void Server_SetRole(AssignedRole role)
    {
        if (!Object.HasStateAuthority) return;
        Role = role;
    }
}
