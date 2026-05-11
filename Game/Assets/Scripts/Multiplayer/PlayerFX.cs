using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    [Header("VFX")]
    public ParticleSystem basicSlash;
    public Transform pointSpawnSlashVFX;
    [Header("SoundEffect")]
    [SerializeField] AudioClip Slash1;
    [SerializeField] AudioClip Slash2;
    public AudioClip[] footSteps;
    [SerializeField] Transform footPlayer;
    public void PlaySlashSound()
    {
        if (Slash1 != null)
        {
            AudioSource.PlayClipAtPoint(Slash1, transform.position);
            
        }
    }
    public void SpawnSlashVFX()
    {
        if (basicSlash != null)
        {
           var obj =  Instantiate(basicSlash, pointSpawnSlashVFX.position, Quaternion.identity);
            Destroy(obj, 2f);
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
}
