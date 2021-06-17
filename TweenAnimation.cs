using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnimation : MonoBehaviour
{
    private Vector3 panelFinalPosition;

    private void Awake()
    {
        panelFinalPosition = gameObject.transform.localScale;
        gameObject.transform.localScale = Vector3.zero;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        LeanTween.scale(gameObject.GetComponent<RectTransform>(), panelFinalPosition, 0.5f);
    }

    public void Close()
    {
        ZoomPan.zoomEnable = true;
        LeanTween.scale(gameObject.GetComponent<RectTransform>(), Vector3.zero, 0.5f);
        StartCoroutine(DisablePanel());
    }

    IEnumerator DisablePanel()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
