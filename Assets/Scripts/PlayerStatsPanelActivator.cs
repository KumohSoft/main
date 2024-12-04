using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsPanelActivator : MonoBehaviour
{
    public GameObject targetPanel; // 활성화/비활성화할 오브젝트
    public KeyCode toggleKey = KeyCode.Tab; // 토글에 사용할 키 (기본값: Tab)

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (targetPanel != null)
            {
                // 활성화 상태를 반대로 변경
                targetPanel.SetActive(!targetPanel.activeSelf);
            }
            else
            {
                Debug.LogWarning("Target Panel is not assigned!");
            }
        }
    }
}
