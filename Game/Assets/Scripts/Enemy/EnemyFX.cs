using UnityEngine;

public class EnemyFX : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Normal EnemySound")]
    public AudioClip WalkSound;
    public AudioClip hitSound;
    public AudioClip DieSound;
    public AudioClip GetHitSound;

    [Header("Boss EnemySound")]
    public AudioClip DragonBossGrawl; //tiếng gừ
    public AudioClip RoarSound; //tiếng gầm
    public AudioClip FireSound; //tiếng phun lửa
    public AudioClip DragonClaw;
    
    [Header("Visual Effect")]
    public GameObject AttackHitEffect;
    public Transform spawnHitVFX;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 20f;
    }

    public void PlayWalkSound()
    {
        if (WalkSound != null)
        {
            AudioSource.PlayClipAtPoint(WalkSound, transform.position, .5f);
        }
    }

    public void PlayHitSound()
    {
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
        SpawnHitVFX(spawnHitVFX.position);
    }

    public void PlayDieSound()
    {
        if (DieSound == null) return;
        GameObject tempAudioObj = new GameObject("TempDieSound_" + gameObject.name);
        tempAudioObj.transform.position = transform.position;
        AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
        tempSource.clip = DieSound;
        tempSource.spatialBlend = 1f;       
        tempSource.minDistance = 2f;        
        tempSource.maxDistance = 20f;      
        tempSource.Play();
        Destroy(tempAudioObj, DieSound.length);
    }

    public void PlayGetHitSound()
    {
        if (GetHitSound != null)
        {
            AudioSource.PlayClipAtPoint(GetHitSound, transform.position);
        }
    }
    
    public void PlayDragonBossGrawl()
    {
        if (DragonBossGrawl != null)
        {
            audioSource.PlayOneShot(DragonBossGrawl);

        }
    }

    public void PlayDragonRoarAndFire()
    {
        if (RoarSound != null) audioSource.PlayOneShot(RoarSound);
        if (FireSound != null) audioSource.PlayOneShot(FireSound);
    }

    public void PlayDragonClaw()
    {
        if (DragonClaw != null)
        {
            audioSource.PlayOneShot(DragonClaw);
        }
    }

    public void SpawnHitVFX(Vector3 hitPoint)
    {
        if (AttackHitEffect != null)
        {
            GameObject vfx = Instantiate(AttackHitEffect, hitPoint, Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }
    public void EnemyDieSound() 
    {
        if (DieSound != null)
        {
            AudioSource.PlayClipAtPoint(DieSound, transform.position);
        }
    }


}
