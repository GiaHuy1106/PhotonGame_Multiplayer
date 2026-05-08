using UnityEngine;

public class GetHit : IState
{
    PlayerContext ctx;
    public GetHit(PlayerContext ctx)
    {
        this.ctx = ctx;
    }

    public void Enter()
    {
        Debug.Log("Enter GetHit");
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }
}
