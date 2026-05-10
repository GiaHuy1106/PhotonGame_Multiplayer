using UnityEngine;

public class NoneState : IState
{
    PlayerContext ctx;
    public NoneState(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }
}
