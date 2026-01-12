using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class StateMachine : MonoBehaviour
{
    private IState current;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
