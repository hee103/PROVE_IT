using System;
using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    [Header("Wander")]
    [SerializeField] public float wanderRadius = 6f;
    [SerializeField] private float wanderStopDistance = 0.4f;

    [Header("Talk (AI trigger)")]
    [SerializeField] private LayerMask talkTargetLayers;
    [SerializeField] private float talkDetectRadius = 2.2f;
    [SerializeField] private float talkChancePerSecond = 0.15f; 
    [SerializeField] private float talkCooldown = 6f;
    [SerializeField] private float faceTargetTurnSpeed = 720f; 

    [Header("Refs")]
    [SerializeField] private NavMeshAgent agent;


    //public event Action<Transform> OnTalkStart;
    //public event Action OnTalkEnd;

    private StateMachine fsm;
    private float nextTalkAllowedTime;

    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        fsm = new StateMachine();

        fsm.ChangeState(new IdleState(this));
    }

    private void Update()
    {
        fsm.Update(Time.deltaTime);
    }


    internal bool CanTryTalk() => Time.time >= nextTalkAllowedTime;

    internal void SetTalkCooldown()
    {
        nextTalkAllowedTime = Time.time + talkCooldown;
    }

    internal bool TryPickTalkTarget(out Transform target)
    {
        target = null;

        var hits = Physics.OverlapSphere(transform.position, talkDetectRadius, talkTargetLayers, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return false;

        float best = float.MaxValue;
        Transform bestT = null;

        for (int i = 0; i < hits.Length; i++)
        {
            var t = hits[i].transform;
            if (!t || t == transform) continue;

            float d = (t.position - transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                bestT = t;
            }
        }

        if (!bestT) return false;

        target = bestT;
        return true;
    }

    internal bool RollTalkChance(float dt)
    {
        float p = 1f - Mathf.Pow(1f - Mathf.Clamp01(talkChancePerSecond), dt);
        return UnityEngine.Random.value < p;
    }


    internal void StopMove()
    {
        if (!agent) return;
        agent.isStopped = true;
        agent.ResetPath();
    }

    internal void ResumeMove()
    {
        if (!agent) return;
        agent.isStopped = false;
    }

    internal bool IsArrived()
    {
        if (!agent) return true;
        if (agent.pathPending) return false;
        if (agent.remainingDistance > Mathf.Max(agent.stoppingDistance, wanderStopDistance)) return false;
        if (agent.hasPath && agent.velocity.sqrMagnitude > 0.01f) return false;
        return true;
    }

    internal bool TrySetRandomDestination(float radius)
    {
        if (!agent) return false;

        for (int i = 0; i < 10; i++)
        {
            Vector3 rand = transform.position + UnityEngine.Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(rand, out var hit, radius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return true;
            }
        }
        return false;
    }

    internal void FaceTarget(Transform target, float dt)
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion to = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, to, faceTargetTurnSpeed * dt);
    }


    internal void ToIdle() => fsm.ChangeState(new IdleState(this));
    internal void ToWander() => fsm.ChangeState(new WanderState(this));
    internal void ToTalk(Transform target) => fsm.ChangeState(new TalkState(this, target));
}