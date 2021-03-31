using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeadState : ZombieState
{
    private static readonly int MovementZHash = Animator.StringToHash("Z_Movement");
    private static readonly int IsDeadHash = Animator.StringToHash("Z_Dead");

    public ZombieDeadState(ZombieComponent zombie, StateMachine stateMachine) : base(zombie, stateMachine)
    {
    }
    
    public override void Start()
    {
        base.Start();
        Z_Component.Z_NavMesh.isStopped = true;
        Z_Component.Z_NavMesh.ResetPath();
        
        Z_Component.Z_Animator.SetFloat(MovementZHash, 0);
        Z_Component.Z_Animator.SetBool(IsDeadHash, true);
    }

    public override void Exit()
    {
        base.Exit();
        Z_Component.Z_NavMesh.isStopped = false;
        Z_Component.Z_Animator.SetBool(IsDeadHash, false);
    }
}
