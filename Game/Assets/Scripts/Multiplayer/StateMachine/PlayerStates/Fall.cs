using UnityEngine;

public class Fall : IState
{
    PlayerContext ctx;
    public Fall(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
    }

    public void Execute(float tick)
    {
        if (ctx.controller.Grounded)
        {
            ctx.player.isJumping = false;
            ctx.ChangeMovementState(nameof(Idle));
        }
    }

    public void Exit()
    {
    }
}
