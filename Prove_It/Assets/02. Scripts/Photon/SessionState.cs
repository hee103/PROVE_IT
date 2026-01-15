using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionState : MonoBehaviour
{
    public Dictionary<PlayerRef, AssignedRole> AssignedRoles = new();
    public Dictionary<PlayerRef, string> Nicknames = new();
}
