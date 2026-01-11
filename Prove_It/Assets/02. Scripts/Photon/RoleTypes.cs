using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RolePreference: byte
{
    Random = 0,
    Inspector = 1,
    Alien = 2
}

public enum AssignedRole: byte
{
    None = 0,
    Inspector = 1,
    Alien = 2
}
