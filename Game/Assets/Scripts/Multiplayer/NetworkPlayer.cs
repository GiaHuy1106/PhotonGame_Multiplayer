using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{

    [SerializeField] Transform _camera;
    [SerializeField] Transform cinemachine;
    [SerializeField] Animator animator;
    [SerializeField] NetworkCharacterController controller;
    [SerializeField] HPHandler hphandler;
    [Networked, OnChangedRender(nameof(OnLocomotionStateChange))]
    public LocomotionState locomotionState { get; set; }
    [Networked, OnChangedRender(nameof(OnCombatStateChanged))]
    public CombatState combatState { get; set; }
    public static NetworkPlayer Local;
    StateMachine locomotion;
    StateMachine combat;
    PlayerContext ctx;
    private void Awake()
    {
        Debug.Log("NetworkPlayer Awake");
        locomotion = new StateMachine();
        combat = new StateMachine();
        ctx = new PlayerContext();
        ctx.SetAnimController(new PlayerAnimatorController(animator))
           .SetHPHandler(hphandler)
           .SetController(controller)
           .SetMovementStateMachine(locomotion)
           .SetCombatStateMachine(combat)
           .AddState(nameof(Idle), new Idle(ctx))
           .AddState(nameof(Move), new Move(ctx))
           .AddState(nameof(Attack01), new Attack01(ctx))
           .AddState(nameof(Attack02), new Attack02(ctx))
           .AddState(nameof(JumpStart), new JumpStart(ctx))
           .AddState(nameof(JumpEnd), new JumpEnd(ctx))
           .AddState(nameof(Fall), new Fall(ctx))
           .AddState(nameof(Defend), new Defend(ctx))
           .AddState(nameof(Die), new Die(ctx))
           .AddState(nameof(NoneState), new NoneState(ctx))
           ;


    }
    public override void Spawned()
    {
        Debug.Log("NetworkPlayer spawned");
        ctx.SetPlayer(this);
        locomotion.ChangeState(ctx.GetState(nameof(Idle)));
        combat.ChangeState(ctx.GetState(nameof(NoneState)));
        if (Object.HasInputAuthority)
        {
            Local = this;
            _camera.gameObject.SetActive(true);
            cinemachine.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Destroy camera");
            Destroy(_camera.gameObject);
        }
    }
    NetworkInputData inputData;

    [Networked]
    public bool isJumping { get; set; }
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority && hphandler.IsDead)
        {
            return;
        }
        if (GetInput(out inputData))
        {
            locomotion.Update(Runner.DeltaTime);
            combat.Update(Runner.DeltaTime);
            ctx.SetInput(inputData);
            if (inputData.isJump && controller.Grounded && !isJumping)
            {
                ctx.ChangeMovementState(nameof(JumpStart));
            }
            if (inputData.isAttack)
            {
                ctx.ChangeCombatState(nameof(Attack01));
            }
            if (inputData.isDefend)
            {
                ctx.ChangeCombatState(nameof(Defend));
            }
        }
        else
        {
            inputData.Reset();
            ctx.SetInput(inputData);
            locomotion.Update(Runner.DeltaTime);
            combat.Update(Runner.DeltaTime);
        }
           
    }
    void OnLocomotionStateChange()
    {
        switch (locomotionState)
        {
            case LocomotionState.Idle:
                ctx.anim.PlayClip(PlayerAnimatorController.IDLE_HASH);
                break;
            case LocomotionState.Move:
                ctx.anim.PlayClip(PlayerAnimatorController.MOVE_HASH);
                break;
            case LocomotionState.JumpStart:
                ctx.anim.PlayClip(PlayerAnimatorController.JUMPSTART_HASH);
                break;
            case LocomotionState.Fall:
                ctx.anim.PlayClip(PlayerAnimatorController.FALL_HASH);
                break;
            case LocomotionState.JumpEnd:
                ctx.anim.PlayClip(PlayerAnimatorController.JUMPEND_HASH);
                break;
            case LocomotionState.Die:
                ctx.anim.PlayClip(PlayerAnimatorController.DIE_HASH);
                break;
            case LocomotionState.DefendHit:
                ctx.anim.PlayClip(PlayerAnimatorController.DEFEND_HASH);
                break;
            case LocomotionState.GetHit:
                ctx.anim.PlayClip(PlayerAnimatorController.GETHIT_HASH);
                break;
            case LocomotionState.GetUp:
                ctx.anim.PlayClip(PlayerAnimatorController.GETUP_HASH);
                break;
            default:
                break;

        }
    }
    void OnCombatStateChanged()
    {
        switch (combatState) 
        {
            case CombatState.NoneState:
                ctx.anim.PlayClip(PlayerAnimatorController.NONE_HASH, 1);
                break;
            case CombatState.Attack01:
                ctx.anim.PlayClip(PlayerAnimatorController.ATTACK01_HASH, 1);
                break;
            case CombatState.Attack02:
                ctx.anim.PlayClip(PlayerAnimatorController.ATTACK02_HASH, 1);
                break;
            case CombatState.Defend:
                ctx.anim.PlayClip(PlayerAnimatorController.DEFEND_HASH, 1);
                break;
            default:
                break;
        }

    }
    public void ChangeLocomotionState(LocomotionState state)
    {
        if(HasStateAuthority)
            locomotionState = state;
    }
    public void ChangeCombatState(CombatState state)
    {
        if (HasStateAuthority)
        {
            combatState = state;
        }
    }
    public override void Render()
    {
        //animator.Play();
    }
}
