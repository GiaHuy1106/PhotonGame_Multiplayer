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
        Debug.Log("Enter Fall");
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
