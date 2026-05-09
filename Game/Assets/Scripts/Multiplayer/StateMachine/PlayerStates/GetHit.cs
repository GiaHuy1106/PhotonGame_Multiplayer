using UnityEngine;

public class GetHit : IState
{
    PlayerContext ctx;
    Animator animator;
    public GetHit(PlayerContext ctx)
    {
        this.ctx = ctx;
    }

    public void Enter()
    {
        if (animator == null)
        {
            animator = ctx.anim.GetAnimator();

        }
    }

    public void Execute(float tick)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(nameof(GetHit)) && stateInfo.normalizedTime >= .95f)
        {
            ctx.ChangeMovementState(nameof(Idle));
        }
    }

    public void Exit()
    {
    }
}
