using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private readonly NpcController npc;
    private float t;
    private float duration;

    public IdleState(NpcController npc) => this.npc = npc;
    public void Enter()
    {
        npc.StopMove();
        t = 0f;
    }
    public void Update(float dt)
    {
        //if (npc.CanTryTalk() && npc.RollTalkChance(dt) && npc.TryPickTalkTarget(out var target))
        //{
        //    npc.ToTalk(target);
        //    return;
        //}

        t += dt;
        if (t >= duration)
            npc.ToWander();
    }
    public void Exit()
    {
        
    }

}
