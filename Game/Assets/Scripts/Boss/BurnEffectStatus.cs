using UnityEngine;
using System.Collections;

public class BurnEffectVisual : MonoBehaviour
{
    private GameObject vfxInstance;

    public void Initialize(float duration, GameObject vfxPrefab, Vector3 offset, Transform parentBone)
    {
        if (vfxPrefab != null)
        {
            Transform attachPoint = (parentBone != null) ? parentBone : transform;
            vfxInstance = Instantiate(vfxPrefab, attachPoint.position + offset, attachPoint.rotation, attachPoint);
        }
        StartCoroutine(BurnRoutine(duration));
    }

    private IEnumerator BurnRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        CleanUp();
    }
    public void CleanUp()
    {
        if (vfxInstance != null) Destroy(vfxInstance);
        Destroy(this);
    }
}