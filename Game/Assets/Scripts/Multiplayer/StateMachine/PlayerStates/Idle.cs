using UnityEngine;

public class Idle : IState
{
    PlayerContext ctx;
    public Idle(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
    }

    public void Execute(float tick)
    {
        if (ctx.inputData.direction != Vector2.zero)
        {
            ctx.ChangeMovementState(nameof(Move));
        }
    }

    public void Exit()
    {

    }
}
