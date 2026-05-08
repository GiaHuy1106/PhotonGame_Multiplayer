using UnityEngine;

public class Defend : IState
{
    PlayerContext ctx;
    public Defend(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    public void Enter()
    {
        Debug.Log("Enter Defend");
        ctx.anim.PlayClip(PlayerAnimatorController.DEFEND_HASH);
    }

    public void Execute(float tick)
    {
    }

    public void Exit()
    {
    }

    
}
