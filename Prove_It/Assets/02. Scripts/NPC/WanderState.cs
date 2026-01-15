using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WanderState : IState
{
    private readonly NpcController npc;

    public WanderState(NpcController npc) => this.npc = npc;
    public void Enter()
    {
        npc.ResumeMove();
        npc.TrySetRandomDestination(npc.wanderRadius);
    }
    public void Update(float dt)
    {
        if (npc.CanTryTalk() && npc.RollTalkChance(dt) && npc.TryPickTalkTarget(out var target))
        {
            npc.ToTalk(target);
            return;
        }

        if (npc.IsArrived())
            npc.ToIdle();
    }

    public void Exit()
    {
       
    }

}
