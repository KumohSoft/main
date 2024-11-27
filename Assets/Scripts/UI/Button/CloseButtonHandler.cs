using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CloseButtonHandler : MonoBehaviour
{
    public PanelHandler popupWindow;

    public void OnButtonClick()
    {
        Vector3 originalScale = transform.localScale;

        var seq = DOTween.Sequence();

        seq.Append(transform.DOScale(originalScale * 0.95f, 0.1f))
            .Append(transform.DOScale(originalScale * 1.05f, 0.1f))
            .Append(transform.DOScale(originalScale * 1f, 0.1f));

        seq.Play().OnComplete(() => {
            popupWindow.Hide();
        });
    }
}
