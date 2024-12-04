using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PopUpEffect : MonoBehaviour
{
    void Start()
    {
        PlayTitleAnimation();
    }

    public void PlayTitleAnimation()
    {
        Vector3 originalScale = transform.localScale;

        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack))
            .Append(transform.DOScale(originalScale * 1.1f, 0.3f).SetEase(Ease.OutQuad));
    }
}
