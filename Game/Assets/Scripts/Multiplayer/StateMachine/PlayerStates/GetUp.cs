using UnityEngine;

public class GetUp : IState
{
    PlayerContext ctx;
    public GetUp(PlayerContext ctx)
    {
        this.ctx = ctx;
    }

    public void Enter()
    {
        Debug.Log("Enter GetUp");
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }
}
