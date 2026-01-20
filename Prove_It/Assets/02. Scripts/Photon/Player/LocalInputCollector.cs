using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInputCollector : MonoBehaviour
{
    public Vector2 Move { get; private set; }   
    public void OnMove(InputAction.CallbackContext ct)
    {
        Move = ct.ReadValue<Vector2>();
    }
}
