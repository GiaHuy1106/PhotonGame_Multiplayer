using UnityEngine;

public class DefendHit : IState
{
    PlayerContext ctx;
    public DefendHit(PlayerContext ctx)
    {
        this.ctx = ctx;
    }
    Animator animator;
    public void Enter()
    {
        if(animator == null)
        {
            animator = ctx.anim.GetAnimator();
        }
    }

    public void Execute(float tick)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsName(nameof(DefendHit)) && stateInfo.normalizedTime >= .95f)
        {
            ctx.ChangeCombatState(nameof(NoneState));
        }
    }

    public void Exit()
    {
    }
}
