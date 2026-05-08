using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{

    [SerializeField] Transform _camera;
    [SerializeField] Transform cinemachine;
    [SerializeField] Animator animator;
    [SerializeField] NetworkCharacterController controller;
    [SerializeField] HPHandler hphandler;
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
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority && hphandler.IsDead)
        {
            return;
        }
        if (GetInput(out NetworkInputData inputData))
        {
            ctx.SetInput(inputData);
            if (inputData.isJump)
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
        locomotion.Update(Runner.DeltaTime);
        combat.Update(Runner.DeltaTime);
    }
    public override void Render()
    {
        //animator.Play();
    }
}
