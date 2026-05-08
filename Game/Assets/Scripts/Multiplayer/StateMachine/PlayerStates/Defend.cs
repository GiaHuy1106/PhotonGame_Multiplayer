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
    }

    public void Execute(float tick)
    {
        if (!ctx.inputData.isDefend)
        {
            ctx.ChangeCombatState(nameof(NoneState));
        }
    }

    public void Exit()
    {
    }

    
}
