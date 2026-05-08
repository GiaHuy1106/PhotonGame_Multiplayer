using System.Collections;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
    [Header("Lightning")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private Transform defaultStrikePoint;
    [SerializeField] private float strikeInterval = 10f;
    [SerializeField] private float lightningLifeTime = 2f;

    [Header("Random Strike Area")]
    [SerializeField] private bool useRandomArea;
    [SerializeField] private Vector3 areaCenter;
    [SerializeField] private Vector3 areaSize = new Vector3(20f, 0f, 20f);

    private Coroutine strikeCoroutine;

    private void OnEnable()
    {
        strikeCoroutine = StartCoroutine(StrikeRoutine());
    }

    private void OnDisable()
    {
        if (strikeCoroutine != null)
            StopCoroutine(strikeCoroutine);
    }

    private IEnumerator StrikeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(strikeInterval);
            StrikeLightning();
        }
    }

    private void StrikeLightning()
    {
        if (lightningPrefab == null)
        {
            Debug.LogWarning("LightningManager: Lightning prefab is missing.");
            return;
        }

        Vector3 strikePosition = GetStrikePosition();
        GameObject lightning = Instantiate(lightningPrefab, strikePosition, Quaternion.identity);

        Debug.Log("LightningManager: Lightning struck at " + strikePosition + ".");

        if (lightningLifeTime > 0f)
            Destroy(lightning, lightningLifeTime);
    }

    private Vector3 GetStrikePosition()
    {
        if (useRandomArea)
        {
            float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
            float z = Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f);
            return areaCenter + new Vector3(x, areaSize.y, z);
        }

        if (defaultStrikePoint != null)
            return defaultStrikePoint.position;

        return transform.position;
    }
}
