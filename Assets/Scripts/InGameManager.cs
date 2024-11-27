using UnityEngine;
using TMPro;

public class InGameManager : MonoBehaviour
{
    public SliderController[] cheeseSliders; // 5개의 SliderController 배열
    public TMP_Text cheeseCounterText; // TextMeshPro 텍스트 연결
    private int completedCheeseCount = 0; // 완료된 치즈 개수

    void Start()
    {
        if (cheeseSliders.Length != 5)
        {
            Debug.LogError("Cheese sliders must contain exactly 5 elements!");
            return;
        }

        foreach (var slider in cheeseSliders)
        {
            slider.OnSliderCompleted += OnCheeseCompleted; // 이벤트 구독
        }

        UpdateCheeseCounter(); // 초기 카운터 업데이트
    }

    private void OnCheeseCompleted()
    {
        completedCheeseCount++; // 완료된 치즈 카운트 증가
        UpdateCheeseCounter();

        if (completedCheeseCount >= 5)
        {
            Debug.Log("All cheeses are completed! Game Finished!");
        }
    }

    private void UpdateCheeseCounter()
    {
        if (cheeseCounterText != null)
        {
            cheeseCounterText.text = $"Cheese Completed: {completedCheeseCount}/5";
        }
        else
        {
            Debug.LogWarning("Cheese counter text is not assigned!");
        }
    }

    void OnDestroy()
    {
        foreach (var slider in cheeseSliders)
        {
            slider.OnSliderCompleted -= OnCheeseCompleted; // 이벤트 구독 해제
        }
    }
}
