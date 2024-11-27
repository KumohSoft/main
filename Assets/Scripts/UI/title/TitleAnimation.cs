using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleAnimation : MonoBehaviour
{
    void Start()
    {
        PlayTitleAnimation();
    }

    public void PlayTitleAnimation()
    {
        Vector3 originalScale = transform.localScale;

        var seq = DOTween.Sequence();

        // 확대했다가 줄이는 애니메이션 추가
        seq.Append(transform.DOScale(originalScale * 1.1f, 0.3f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(originalScale, 0.5f).SetEase(Ease.InQuad))
            .SetLoops(-1, LoopType.Restart); // 무한 루프
    }
}
