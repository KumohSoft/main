using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelHandler : MonoBehaviour
{
    void Start()
    {
        DOTween.Init();
        // transform의 scale 값을 모두 0.1f로 변경
        transform.localPosition = Vector3.one * 0.1f;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.localPosition = Vector3.one * 0.1f;

        var seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.1f, 0.2f))
            .Append(transform.DOScale(1f, 0.1f));

        seq.Play();
    }

    public void Hide()
    {
        Vector3 originalScale = transform.localScale;

        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(originalScale * 1.1f, 0.1f))
            .Append(transform.DOScale(originalScale * 0.2f, 0.2f));

        seq.Play().OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
