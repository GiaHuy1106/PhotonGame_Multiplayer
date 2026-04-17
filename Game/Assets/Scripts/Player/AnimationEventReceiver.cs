using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [Header("VFX References")]
    public ParticleSystem slashVFX;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        if (slashVFX != null)
        {
            slashVFX.Stop();
        }
    }
    public void StartSlash()
    {
        if (slashVFX != null)
        {
            slashVFX.Play();
        }
    }
    public void EndSlash()
    {
        if (slashVFX != null)
        {
            slashVFX.Stop();
        }
    }
    public void FinishAttack()
    {
        if (playerMovement != null)
        {
            playerMovement.ResetAttack();
        }
    }
}