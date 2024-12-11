using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class CloseButtonHandler : MonoBehaviour
{
    public PanelHandler popupWindow;
    public InputField input1,input2,input3,input4;
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
        input1.text = "";
        input2.text = "";
        input3.text = "";
        input4.text = "";
    }
}
