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
        Debug.Log("Enter NoneState");
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }
}
