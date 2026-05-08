using UnityEngine;

public class JumpStart : IState
{
    PlayerContext ctx;
    public JumpStart(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        ctx.controller.Jump();
        Debug.Log("Enter JumpStart");
        Debug.Log("Velocity: " + ctx.controller.Velocity);
    }

    public void Execute(float tick)
    {
        Debug.Log(ctx.controller.Velocity);
        if(ctx.controller.Velocity.y <= 0f && !ctx.controller.Grounded)
        {
            ctx.ChangeMovementState(nameof(Fall));
        }
    }

    public void Exit()
    {
    }
}
