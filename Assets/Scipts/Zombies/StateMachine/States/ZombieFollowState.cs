using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFollowState : ZombieState
{
    private readonly GameObject FollowTarget;
    private static readonly int MovementZHash = Animator.StringToHash("Z_Movement");
    private const float StopDistance = 1.5f;
    
    public ZombieFollowState(GameObject followTarget, ZombieComponent zombie, StateMachine stateMachine) : base(zombie, stateMachine)
    {
        FollowTarget = followTarget;
        UpdateInterval = 2.0f;
    }
    
    public override void Start()
    {
        base.Start();
        Z_Component.Z_NavMesh.SetDestination(FollowTarget.transform.position);

    }

    public override void IntervalUpdate()
    {
        base.IntervalUpdate();
        Z_Component.Z_NavMesh.SetDestination(FollowTarget.transform.position);
    }

    public override void Update()
    {
        base.Update();
        Z_Component.Z_Animator.SetFloat(MovementZHash, Z_Component.Z_NavMesh.velocity.normalized.z);
        
        float distanceBetween = Vector3.Distance(Z_Component.transform.position, FollowTarget.transform.position);
       if (distanceBetween< StopDistance)
       {
           StateMachine.ChanceState(ZombieStateType.Attack);
       }
       
        
    }
}
