using System.Collections;
using TMPro;
using UnityEngine;

public class floatingText : MonoBehaviour
{
    public TMP_Text floatingTextUI;
    public float showDelay = 2f;
    public float floatDistance = 1f;
    public float floatDuration = 1f;

    private bool isPlayerInside;
    private Coroutine floatingRoutine;
    private Vector3 initialLocalTextPosition;

    void Start()
    {
        if (floatingTextUI == null) return;

        initialLocalTextPosition = floatingTextUI.transform.localPosition;
        floatingTextUI.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || floatingTextUI == null) return;

        isPlayerInside = true;

        if (floatingRoutine != null)
        {
            StopCoroutine(floatingRoutine);
        }

        floatingRoutine = StartCoroutine(FloatingAnim());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || floatingTextUI == null) return;

        isPlayerInside = false;
        floatingTextUI.gameObject.SetActive(false);
        floatingTextUI.transform.localPosition = initialLocalTextPosition;

        if (floatingRoutine != null)
        {
            StopCoroutine(floatingRoutine);
            floatingRoutine = null;
        }
    }

    IEnumerator FloatingAnim()
    {
        yield return new WaitForSeconds(showDelay);

        if (!isPlayerInside || floatingTextUI == null)
        {
            yield break;
        }

        floatingTextUI.transform.localPosition = initialLocalTextPosition;
        floatingTextUI.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Vector3 startPos = initialLocalTextPosition;
        Vector3 endPos = startPos + new Vector3(0f, floatDistance, 0f);

        while (elapsedTime < floatDuration)
        {
            floatingTextUI.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / floatDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        floatingTextUI.transform.localPosition = endPos;
        floatingRoutine = null;
    }
}
