



public class ZombieState : State
{
    protected ZombieComponent Z_Component;
    public ZombieState(ZombieComponent zombie, StateMachine stateMachine) : base(stateMachine)
    {
        Z_Component = zombie;
    }
}

public enum ZombieStateType
{
    Idle,
    Attack,
    Follow,
    Dead
}