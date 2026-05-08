using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimatorController
{
    Animator anim;
    //public static int MOVE_HASH = Animator.StringToHash("Move");
    //public static int ATTACK01_HASH = Animator.StringToHash("Attack01");
    //public static int ATTACK02_HASH = Animator.StringToHash("Attack02");
    //public static int IDLE_HASH = Animator.StringToHash("Idle");
    //public static int JUMPSTART_HASH = Animator.StringToHash("JumpStart");
    //public static int JUMPEND_HASH = Animator.StringToHash("JumpEnd");
    //public static int FALL_HASH = Animator.StringToHash("Fall");
    //public static int DEFEND_HASH = Animator.StringToHash("Defend");
    //public static int DEFENDHIT_HASH = Animator.StringToHash("DefendHit");
    //public static int GETHIT_HASH = Animator.StringToHash("GetHit");
    //public static int DIE_HASH = Animator.StringToHash("Die");
    //public static int GETUP_HASH = Animator.StringToHash("GetUp");
    //public static int NONE_HASH = Animator.StringToHash("NoneStae");
    public static int MOVE_HASH = Animator.StringToHash(nameof(Move));
    public static int ATTACK01_HASH = Animator.StringToHash(nameof(Attack01));
    public static int ATTACK02_HASH = Animator.StringToHash(nameof(Attack02));
    public static int IDLE_HASH = Animator.StringToHash(nameof(Idle));
    public static int JUMPSTART_HASH = Animator.StringToHash(nameof(JumpStart));
    //public static int JUMPEND_HASH = Animator.StringToHash(nameof(JumpEnd));
    public static int FALL_HASH = Animator.StringToHash(nameof(Fall));
    public static int DEFEND_HASH = Animator.StringToHash(nameof(Defend));
    public static int DEFENDHIT_HASH = Animator.StringToHash(nameof(DefendHit));
    public static int GETHIT_HASH = Animator.StringToHash(nameof(GetHit));
    public static int DIE_HASH = Animator.StringToHash(nameof(Die));
    public static int GETUP_HASH = Animator.StringToHash(nameof(GetUp));
    public static int NONE_HASH = Animator.StringToHash(nameof(NoneState));
    public PlayerAnimatorController(Animator anim)
    {
        this.anim = anim;
    }

    public void PlayClip(int hash, int layer = 0)
    {
        if (anim != null)
            anim.Play(hash, layer);

    }

    public Animator GetAnimator()
    {
        return anim;
    }

    /// <summary>
    ///     Play a clip on top of the current animation, this is used for attack combos, so that the player can transition from one attack to another without having to wait for the first attack to finish.
    /// </summary>
    /// <param name="current">hash of current clip</param>
    /// <param name="overLay">hash of new clip </param>
    public void PlayClipOverLay(int current, int overLay)
    {
        anim.Play(overLay);
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float duration = stateInfo.length;
        Utils.DelayCall(duration, () => PlayClip(current));       
    }




}
