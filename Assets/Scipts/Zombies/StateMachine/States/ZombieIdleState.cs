using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : ZombieState
{
    private static readonly int MovementZ = Animator.StringToHash("Z_Movement");

    public ZombieIdleState(ZombieComponent zombie, StateMachine stateMachine) : base(zombie, stateMachine)
    {

    }
    public override void Start()
    {
        base.Start();
        Z_Component.Z_NavMesh.isStopped = true;
        Z_Component.Z_NavMesh.ResetPath();
        Z_Component.Z_Animator.SetFloat(MovementZ, 0.0f);

    }
}
