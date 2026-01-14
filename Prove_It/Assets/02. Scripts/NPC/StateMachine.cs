using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IState
{
    void Enter();
    void Update(float dt);
    void Exit();
}

public class StateMachine
{
    private IState current;

    public void ChangeState(IState next)
    {
        if (next == null) return;

        current?.Exit();
        current = next;
        current.Enter();
    }

    public void Update(float dt)
    {
        current?.Update(dt);
    }
}
