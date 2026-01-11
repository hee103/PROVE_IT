using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public static class RoleAssigner
{
    private static Random Rng = new();

    public static void AssignRoles_Server(Dictionary<PlayerRef, NetworkObject> spawnedPlayers)
    {
        var list = spawnedPlayers.Values
            .Select(o => o != null ? o.GetComponent<NetworkPlayer>(): null)
            .Where(p => p != null).ToList();

        if (list.Count < 2) return;

        var a = list[0];
        var b = list[1];

        var roleA = DecideRoleForFirst(a.Preference, b.Preference);
        var roleB = (roleA == AssignedRole.Inspector) ? AssignedRole.Alien : AssignedRole.Inspector;

        a.Server_AssignRole(roleA);
        b.Server_AssignRole(roleB);
    }

    private static AssignedRole DecideRoleForFirst(RolePreference a , RolePreference b)
    {
        // 둘 다 랜덤
        if (a == RolePreference.Random && b == RolePreference.Random)
            return Rng.Next(0, 2) == 0 ? AssignedRole.Inspector : AssignedRole.Alien;
        
        //한쪽만 명시, 다른쪽 랜덤
        if (a == RolePreference.Inspector && b == RolePreference.Random) return AssignedRole.Inspector;
        if (a == RolePreference.Alien && b == RolePreference.Random) return AssignedRole.Alien;

        if (a == RolePreference.Random && b == RolePreference.Inspector) return AssignedRole.Alien;
        if (a == RolePreference.Random && b == RolePreference.Alien) return AssignedRole.Inspector;
        
        //둘다 같은 선택
        if(a == b)
        {
            //첫번째를 Inspector로 강제
            if (a == RolePreference.Inspector)
                return AssignedRole.Inspector;

            if (a == RolePreference.Alien)
                return AssignedRole.Inspector;
        }

        //서로 다른 선택
        return (a == RolePreference.Inspector) ? AssignedRole.Inspector : AssignedRole.Alien;
    }
}
