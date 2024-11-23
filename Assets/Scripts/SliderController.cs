using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider; // 연결된 Slider
    public float duration = 30f; // Slider가 채워지는 데 걸리는 시간
    private float elapsedTime = 0f; // 경과 시간
    private bool isIncreasing = false; // Slider가 올라가는 상태
    private bool isDecreasing = false; // Slider가 내려가는 상태
    private bool isCompleted = false; // Slider가 완료된 상태
    public delegate void SliderCompleted(); // Slider 완료 이벤트
    public event SliderCompleted OnSliderCompleted;

    private CanvasGroup canvasGroup; // CanvasGroup을 사용해 숨김 처리

    void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider가 연결되지 않았습니다.");
            return;
        }

        slider.value = 0f; // 초기값 설정
        slider.maxValue = 1f; // 최대값 설정

        // CanvasGroup 추가 또는 참조
        canvasGroup = slider.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = slider.gameObject.AddComponent<CanvasGroup>();
        }

        HideSlider(); // 시작 시 Slider 숨김
    }

    void Update()
    {
        if (isIncreasing)
        {
            if (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                slider.value = elapsedTime / duration;
                ShowSlider(); // 증가 중에는 Slider 표시
            }
            else
            {
                CompleteSlider();
            }
        }
        else if (isDecreasing)
        {
            if (slider.value > 0)
            {
                slider.value -= Time.deltaTime / duration;
                // Slider는 감소 중에도 계속 숨겨진 상태 유지
            }
            else
            {
                StopDecreasing();
            }
        }
    }

    public void StartIncreasing()
    {
        if (isCompleted)
        {
            Debug.Log($"{gameObject.name} slider is already completed.");
            return; // 완료된 상태라면 활성화 금지
        }

        isIncreasing = true;
        isDecreasing = false;
        elapsedTime = slider.value * duration; // 진행 중인 상태에서 재개
        ShowSlider(); // Slider 표시
        Debug.Log($"{gameObject.name} slider is now increasing.");
    }

    public void StartDecreasing()
    {
        if (isCompleted)
        {
            Debug.Log($"{gameObject.name} slider is already completed.");
            return; // 완료된 상태라면 비활성화 금지
        }

        isDecreasing = true;
        isIncreasing = false;
        HideSlider(); // Slider 숨김
    }

    private void CompleteSlider()
    {
        isIncreasing = false;
        isCompleted = true; // 완료 상태로 설정
        HideSlider(); // 완료 시 Slider 숨김

        OnSliderCompleted?.Invoke(); // 완료 이벤트 호출
        Debug.Log($"{gameObject.name} slider completed!");
    }

    private void StopDecreasing()
    {
        isDecreasing = false;
        HideSlider(); // Slider 숨김
    }

    private void ShowSlider()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1; // Slider 표시
            canvasGroup.blocksRaycasts = true; // 상호작용 허용
        }
    }

    private void HideSlider()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0; // Slider 숨김
            canvasGroup.blocksRaycasts = false; // 상호작용 차단
        }
    }

    public bool IsCompleted()
    {
        return isCompleted; // isCompleted 변수 값 반환
    }
}
