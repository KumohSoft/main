using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEffectHandler : MonoBehaviour
{
    private Vector3 originalScale;

    private void Start()
    {
        // 초기 크기를 저장
        originalScale = transform.localScale;
    }

    public void OnButtonClick()
    {
        // 애니메이션 실행 전에 버튼 크기를 초기 크기로 복원
        transform.localScale = originalScale;

        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(originalScale * 0.95f, 0.1f))
            .Append(transform.DOScale(originalScale * 1.05f, 0.1f))
            .Append(transform.DOScale(originalScale, 0.1f));

        seq.Play();
    }
}