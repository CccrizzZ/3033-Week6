using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StateMachine))]
public class ZombieComponent : MonoBehaviour
{
    public NavMeshAgent Z_NavMesh {get; private set;}

    public Animator Z_Animator {get; private set;}

    public StateMachine StateMachine { get; private set; }

    public GameObject FollowTarget;


    [SerializeField]
    bool Debug;


    void Awake()
    {
        Z_NavMesh = GetComponent<NavMeshAgent>();
        Z_Animator = GetComponent<Animator>();
        StateMachine = GetComponent<StateMachine>();


    }


    void Start()
    {
        if (Debug)
        {
            Init(FollowTarget);
        }
    }


    void Update()
    {
    }


    void Init(GameObject followTarget)
    {
        FollowTarget = followTarget;

        ZombieIdleState idleState = new ZombieIdleState(this, StateMachine);
        StateMachine.AddState(ZombieStateType.Idle, idleState);

        ZombieFollowState followState = new ZombieFollowState(followTarget, this, StateMachine);
        StateMachine.AddState(ZombieStateType.Follow, followState);

        ZombieAttackState attackState = new ZombieAttackState(followTarget, this, StateMachine);
        StateMachine.AddState(ZombieStateType.Attack, attackState);

        ZombieDeadState deadState = new ZombieDeadState(this, StateMachine);
        StateMachine.AddState(ZombieStateType.Dead, deadState);

        StateMachine.Initialize(ZombieStateType.Follow);
    }


}
