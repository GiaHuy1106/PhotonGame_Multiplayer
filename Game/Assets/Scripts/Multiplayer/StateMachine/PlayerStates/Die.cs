using UnityEngine;

public class Die : IState
{
    PlayerContext ctx;
    public Die(PlayerContext ctx)
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
