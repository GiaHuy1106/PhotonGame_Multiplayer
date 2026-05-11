using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    [Header("VFX")]
    public ParticleSystem basicSlash;
    public ParticleSystem recover;
    [Header("SoundEffect")]
    [SerializeField] AudioClip Slash1;
    [SerializeField] AudioClip Slash2;
    public AudioClip[] footSteps;
    [SerializeField] Transform footPlayer;
    [SerializeField] AudioClip healingMagic;
    private void Awake()
    {
        GetComponentInParent<HPHandler>().OnHealth += PlayerFX_OnHealth;

    }

    private void PlayerFX_OnHealth()
    {
        Debug.Log("OnHealth");
        PlayRecover();
    }

    public void PlaySlashSound()
    {
        if (Slash1 != null)
        {
            AudioSource.PlayClipAtPoint(Slash1, transform.position);
            
        }
    }
    public void PlaySlash()
    {
        if (basicSlash != null)
        {
            basicSlash.Play();
        }
        
    }
    public void PlaySlashSound2()
    {
        if (Slash2 != null)
        {
            AudioSource.PlayClipAtPoint(Slash2, transform.position);
        }
    }
    public void PlayFootstep()
    {
        if ( footSteps.Length > 0)
        {
            int index = Random.Range(0, footSteps.Length);
            AudioSource.PlayClipAtPoint(footSteps[index], footPlayer.position);
        }
    }

    public void PlayRecover()
    {
        if (recover != null)
        {
            recover.Play();
        }
        PlayHealingSound();
    }
    public void PlayHealingSound()
    {
        if (healingMagic != null)
        {
            AudioSource.PlayClipAtPoint(healingMagic, transform.position);
        }
    }
}
