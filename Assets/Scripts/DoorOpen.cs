using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class DoorOpen : MonoBehaviour
{
    public float moveSpeed = 2f; // Grid가 이동하는 속도
    public float targetYOffset = 50f; // 목표 y 좌표 상승량
    public Text messageText; // UI 텍스트 컴포넌트를 연결합니다.

    private Transform gridTransform;

    private void Start()
    {
        // Grid라는 자식 오브젝트를 찾습니다.
        gridTransform = transform.Find("Grid");
        if (gridTransform != null)
        {
            Open();
        }
        else
        {
            Debug.LogError("Grid child not found!");
        }
    }

    void Open()
    {
        ShowMessage("문이 열렸습니다!\n탈출하세요!");
        StartCoroutine(OpenDoorCoroutine());
    }

    IEnumerator OpenDoorCoroutine()
    {
        float initialY = gridTransform.localPosition.y;
        float targetY = initialY + targetYOffset;

        // y 좌표를 천천히 상승
        while (gridTransform.localPosition.y < targetY)
        {
            Vector3 newPosition = gridTransform.localPosition;
            newPosition.y += moveSpeed * Time.deltaTime;
            newPosition.y = Mathf.Min(newPosition.y, targetY); // 목표 좌표를 초과하지 않도록 제한
            gridTransform.localPosition = newPosition;

            yield return null; // 다음 프레임까지 대기
        }
    }
    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message; // 메시지 설정
            messageText.gameObject.SetActive(true); // 메시지 표시

            // 일정 시간 후 메시지를 숨깁니다.
            StartCoroutine(HideMessageAfterDelay(3f)); // 3초 후 숨김
        }
    }

    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.gameObject.SetActive(false);
    }
}
