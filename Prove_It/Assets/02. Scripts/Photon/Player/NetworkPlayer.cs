using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public NetworkString<_32> Nickname { get; private set; }
    [Networked] public RolePreference Preference { get; private set; }
    [Networked] public AssignedRole Role { get; private set; }

    //서버 기본값
    public void Server_InitDefults()
    {
        if (!Object.HasInputAuthority) return;
        Nickname = "Player";
        Preference = RolePreference.Random;
        Role = AssignedRole.None;
    }

    //로비에서 클라가 서버에 제출
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SubmitProfile(string nickname, RolePreference pref)
    {
        Nickname = string.IsNullOrWhiteSpace(nickname) ? "Player" : nickname.Trim();
        Preference = pref;
    }

    //서버 역할 확정
    public void Server_AssignRole(AssignedRole role)
    {
        if (!Object.HasStateAuthority) return;
        Role = role;
    }

}
