using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkState : IState
{
    private readonly NpcController npc;
    private readonly Transform target;
    private bool started;


    public TalkState(NpcController npc, Transform target)
    {
        this.npc = npc;
        this.target = target;
    }
    public void Enter()
    {
        npc.StopMove();
        npc.SetTalkCooldown();

        started = true;
        //npc.OnTalkStart?.Invoke(target);

    }

    public void Update(float dt)
    {
        npc.FaceTarget(target, dt);


    }


    public void Exit()
    {
        if (!started) return;
    }

   

}
