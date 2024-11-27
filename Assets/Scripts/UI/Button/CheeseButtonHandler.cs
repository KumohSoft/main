using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CheeseButtonHandler : MonoBehaviour
{
    public PanelHandler popupWindow;

    public void OnButtonClick()
    {
        Vector3 originalScale = transform.localScale;

        // 버튼 애니메이션
        var buttonSeq = DOTween.Sequence();
        buttonSeq.Append(transform.DOScale(originalScale * 0.95f, 0.1f))
                 .Append(transform.DOScale(originalScale * 1.05f, 0.1f))
                 .Append(transform.DOScale(originalScale, 0.1f))
                 .OnComplete(() =>
                 {
                     // 팝업 창 표시
                     popupWindow.Show();

                     // 버튼 비활성화 애니메이션
                     var hideSeq = DOTween.Sequence();
                     hideSeq.Append(transform.DOScale(originalScale * 0.2f, 0.2f))
                            .OnComplete(() =>
                            {
                                gameObject.SetActive(false); // 버튼 비활성화
                            });
                 });

        buttonSeq.Play();
    }
}

