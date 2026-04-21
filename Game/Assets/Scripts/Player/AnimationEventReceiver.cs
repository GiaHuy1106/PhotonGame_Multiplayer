using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [Header("VFX References")]
    public ParticleSystem slashVFX;

    private PlayerMovement playerMovement;
    public SwordDamage swordScript;
    [Header("Audio References")]
    public AudioSource footstepAudioSource;
    public AudioSource weaponAudioSource;   

    [Header("Audio Clips")]
    public AudioClip[] footstepClips; 
    public AudioClip slashClip;      

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
        swordScript.EnableDamage();
        if (slashVFX != null)
        {
            slashVFX.Play();
        }
        if (weaponAudioSource != null && slashClip != null)
        {
            weaponAudioSource.PlayOneShot(slashClip);
        }
    }
    public void EndSlash()
    {
        swordScript.DisableDamage();
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
    public void PlayFootstep()
    {
        if (footstepAudioSource != null && footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            footstepAudioSource.PlayOneShot(footstepClips[index]);
        }
    }
}